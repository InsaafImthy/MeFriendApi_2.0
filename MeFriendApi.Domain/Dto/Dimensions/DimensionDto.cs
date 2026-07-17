using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.Dimensions
{
    public class DimensionDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("dimensionvalues")]
        public List<DimensionValueDto> DimensionValues { get; set; } = new();

    }
}
