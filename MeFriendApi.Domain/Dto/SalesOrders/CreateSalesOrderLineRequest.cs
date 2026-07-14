using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class CreateSalesOrderLineRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("unitpriceexclTax")]
        public decimal UnitPriceExclTax { get; set; }
    }
}
