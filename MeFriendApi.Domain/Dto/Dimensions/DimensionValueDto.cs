using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.Dimensions
{
    public class DimensionValueDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("dimensionCode")]
        public string? DimensionCode { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
