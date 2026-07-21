using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto
{
    public class CustomerLookupDto
    {
        [JsonPropertyName("number")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
