using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class SalesOrderLineDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("documentNo")]
        public string? DocumentNo { get; set; }

        [JsonPropertyName("sequence")]
        public int? Sequence { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }

        [JsonPropertyName("rate")]
        public decimal? Rate { get; set; }

        [JsonPropertyName("lineDiscountPercentage")]
        public decimal? LineDiscountPercentage { get; set; }

        [JsonPropertyName("lineAmountExclVat")]
        public decimal? LineAmountExclVat { get; set; }

        [JsonPropertyName("invoiceDiscountAmountExclVat")]
        public decimal? InvoiceDiscountAmountExclVat { get; set; }
    }
}
