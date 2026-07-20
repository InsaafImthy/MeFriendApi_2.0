using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.Salespersons
{
    public class SalespersonLookupDto
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
