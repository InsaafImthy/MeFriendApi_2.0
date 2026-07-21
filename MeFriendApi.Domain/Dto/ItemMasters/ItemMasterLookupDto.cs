using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto
{
    public class ItemMasterLookupDto
    {
        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
