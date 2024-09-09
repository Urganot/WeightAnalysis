using System.Text.Json.Serialization;

namespace WeightAnalysis.Models
{
    public class RequestTokenResponseBody
    {
        public string userid { get; set; }

        [JsonPropertyName("refresh_token")]
        public string refreshToken { get; set; }

        [JsonPropertyName("access_token")]
        public string accessToken { get; set; }
        public string scope { get; set; }
        [JsonPropertyName("expires_in")]
        public int expiresIn { get; set; }
    }

    public class RequestTokenResponse
    {
        public int status { get; set; }
        [JsonPropertyName("body")]
        public RequestTokenResponseBody Body { get; set; }
    }
}
