using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using System.Text.Json;
using System.Text;
using WeightAnalysis.Components.Pages;

namespace WeightAnalysis.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using WeightAnalysis.Controllers;

    public class GetWeightData
    {


        public static List<MeasurementData> extractWeightData(MeasurementResponse responseData)
        {
            return responseData.body.MeasureGroups
                .SelectMany(x =>
                    x.Measurements.Select(m =>
                        new MeasurementData()
                        {
                            Timestamp = DateTimeOffset.FromUnixTimeSeconds(x.created).DateTime,
                            Value = m.value / Math.Pow(10, Math.Abs(m.unit))
                        }
                    )
                ).ToList();
        }

        public static string getWeightData(IDbContextFactory<WeightContext> contextFactory)
        {
            var uri = "https://wbsapi.withings.net/measure";

            using (var dbContext = contextFactory.CreateDbContext())
            {
                var user = dbContext.Users.First();

                var response = RequestDataFromEndpoint(user, uri, contextFactory);

                response.EnsureSuccessStatusCode();

                var content = response.Content.ReadAsStringAsync().Result;
                MeasurementResponse? responseData = JsonSerializer.Deserialize<MeasurementResponse>(content);

                var extractedData = extractWeightData(responseData);

                var transformedData = TransformDataForExcel(extractedData);

                return ConvertToCsv(transformedData);
            }
        }

        private static List<MeasurementData> TransformDataForExcel(List<MeasurementData> extractedData)
        {
            return extractedData
                .GroupBy(x => x.Timestamp.Date)
                .Select(g => new MeasurementData(g.Key, g.Min(x => x.Value)))
                .OrderBy(x => x.Timestamp).ToList();
        }

        private static HttpResponseMessage RequestDataFromEndpoint(User user, string uri, IDbContextFactory<WeightContext> dbContextFactory)
        {
            var client = new HttpClient();

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "action", "getmeas" },
                {"meastype", ((int)MeasurementTypes.Weight).ToString()}
            };

            var accessToken = Odata.GetValidAccessToken(user, dbContextFactory);

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + accessToken }
            };

            var body = new FormUrlEncodedContent(data);

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = client.PostAsync(uri, body).Result;
            return response;
        }

        private static string ConvertToCsv(List<MeasurementData> transformedData)
        {
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ";"
            };

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(transformedData);
            }

            return sb.ToString();
        }
    }
}
