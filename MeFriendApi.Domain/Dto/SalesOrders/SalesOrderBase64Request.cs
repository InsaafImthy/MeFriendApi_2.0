using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class SalesOrderBase64Request
    {
        [JsonPropertyName("base64")]
        public string? Base64 { get; set; }
    }
}
