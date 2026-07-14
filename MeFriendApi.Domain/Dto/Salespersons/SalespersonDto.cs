using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.Salespersons
{
    public class SalespersonDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("phoneNo")]
        public string? PhoneNo { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
