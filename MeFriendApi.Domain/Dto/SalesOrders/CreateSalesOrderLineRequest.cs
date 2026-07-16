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

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [JsonPropertyName("dimension")]
        public List<CreateSalesOrderLineDimensionRequest> Dimension { get; set; } = new();
    }

    public class CreateSalesOrderLineDimensionRequest
    {
        [JsonPropertyName("dimensionCode")]
        public string? DimensionCode { get; set; }

        [JsonPropertyName("dimensionValueCode")]
        public string? DimensionValueCode { get; set; }
    }
}
