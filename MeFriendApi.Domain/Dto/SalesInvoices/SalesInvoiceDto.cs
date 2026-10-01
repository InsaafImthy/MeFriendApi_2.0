using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesInvoices
{
    public class SalesInvoiceDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }


        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("invoiceNo")]
        public string? InvoiceNo { get; set; }

        [JsonPropertyName("invoiceDate")]
        public string? InvoiceDate { get; set; }

        [JsonPropertyName("customerCode")]
        public string? CustomerCode { get; set; }

        [JsonPropertyName("customerName")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("customerAddress")]
        public string? CustomerAddress { get; set; }

        [JsonPropertyName("customerGSTNo")]
        public string? CustomerGSTNo { get; set; }

        [JsonPropertyName("clientCode")]
        public string? ClientCode { get; set; }

        [JsonPropertyName("clientName")]
        public string? ClientName { get; set; }

        [JsonPropertyName("clientAddress")]
        public string? ClientAddress { get; set; }

        [JsonPropertyName("clientGSTNo")]
        public string? ClientGSTNo { get; set; }

        [JsonPropertyName("salesPerson")]
        public string? SalesPerson { get; set; }

        [JsonPropertyName("tradeDiscount")]
        public decimal? TradeDiscount { get; set; }

        [JsonPropertyName("sgst")]
        public decimal? Sgst { get; set; }

        [JsonPropertyName("cgst")]
        public decimal? Cgst { get; set; }

        [JsonPropertyName("igst")]
        public decimal? Igst { get; set; }

        [JsonPropertyName("netAmount")]
        public decimal? NetAmount { get; set; }

        [JsonPropertyName("salesInvoiceLines")]
        public List<SalesInvoiceLineDto> SalesInvoiceLines { get; set; } = new();
    }
}
