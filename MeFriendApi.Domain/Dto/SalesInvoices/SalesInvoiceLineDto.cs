using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesInvoices
{
    public class SalesInvoiceLineDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("documentNo")]
        public string? DocumentNo { get; set; }

        [JsonPropertyName("lineNo")]
        public int? LineNo { get; set; }

        [JsonPropertyName("itemNo")]
        public string? ItemNo { get; set; }

        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }

        [JsonPropertyName("HSNCode")]
        public string? HsnCode { get; set; }

        [JsonPropertyName("GSTRate")]
        public string? GstRate { get; set; }

        [JsonPropertyName("Amount")]
        public decimal? Amount { get; set; }
    }
}
