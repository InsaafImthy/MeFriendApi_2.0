using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class SalesOrderPostResponse
    {
        [JsonPropertyName("base64")]
        public string? Base64 { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
