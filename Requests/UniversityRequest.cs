using System.Text.Json.Serialization;

namespace MyIgniteApi.Requests
{
    public class UniversityRequest
    {
        [JsonPropertyName("countries")]
        public List<string>? Countries { get; set; }
    }
}
