namespace WeightAnalysis.Models
{
    using System.Text.Json.Serialization;

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Body
    {
        public int updatetime { get; set; }
        public string timezone { get; set; }

        [JsonPropertyName("measuregrps")]
        public List<MeasureGroup> MeasureGroups{ get; set; }
    }

    public class Measurement
    {
        public int value { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
        public int unit { get; set; }
        public int algo { get; set; }
        public int fm { get; set; }
    }

    public enum MeasurementTypes
    {
    	Weight=1,
    	HeightM=4,
    	FatFreeMassKg = 5,
    	FatRatioPercent=6,
    	FatMassWeightKg=8
    }

    public class MeasureGroup
    {
        [JsonPropertyName("grpid")]
        public long GroupId { get; set; }
        public int attrib { get; set; }
        public int date { get; set; }
        public int created { get; set; }
        public int modified { get; set; }
        public int category { get; set; }
        public string deviceid { get; set; }
        public string hash_deviceid { get; set; }
        [JsonPropertyName("measures")]
        public List<Measurement> Measurements { get; set; }
        public int? modelid { get; set; }
        public string model { get; set; }
        public object comment { get; set; }
    }

    public class MeasurementResponse
    {
        public int status { get; set; }
        public Body body { get; set; }
    }



}
