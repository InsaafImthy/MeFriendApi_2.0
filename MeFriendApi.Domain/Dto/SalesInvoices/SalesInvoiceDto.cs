using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesInvoices
{
    public class SalesInvoiceDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("base64")]
        public string? Base64 { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("invoiceNo")]
        public string? InvoiceNo { get; set; }

        [JsonPropertyName("sellToCustomerNo")]
        public string? SellToCustomerNo { get; set; }

        [JsonPropertyName("sellToCustomerName")]
        public string? SellToCustomerName { get; set; }

        [JsonPropertyName("salesOrderNo")]
        public string? SalesOrderNo { get; set; }

        [JsonPropertyName("postingDate")]
        public string? PostingDate { get; set; }

        [JsonPropertyName("dueDate")]
        public string? DueDate { get; set; }

        [JsonPropertyName("totalAmount")]
        public decimal? TotalAmount { get; set; }

        [JsonPropertyName("paidAmount")]
        public decimal? PaidAmount { get; set; }

        [JsonPropertyName("outstandingAmount")]
        public decimal? OutstandingAmount { get; set; }

        [JsonPropertyName("paymentStatus")]
        public string? PaymentStatus { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("SalesInvoiceLines")]
        public List<SalesInvoiceLineDto> SalesInvoiceLines { get; set; } = new();

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
