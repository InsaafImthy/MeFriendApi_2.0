using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class SalesOrderLineDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("documentNo")]
        public string? DocumentNo { get; set; }

        [JsonPropertyName("lineNo")]
        public int? LineNo { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("unitpriceexclTax")]
        public decimal? UnitPriceExclTax { get; set; }

        [JsonPropertyName("lineAmount")]
        public decimal? LineAmount { get; set; }

        [JsonPropertyName("amountIncludingVAT")]
        public decimal? AmountIncludingVAT { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
