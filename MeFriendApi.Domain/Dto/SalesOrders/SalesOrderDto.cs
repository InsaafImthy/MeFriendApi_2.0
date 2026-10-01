using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class SalesOrderDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("postingDate")]
        public string? PostingDate { get; set; }

        [JsonPropertyName("sellToCustomerNo")]
        public string? SellToCustomerNo { get; set; }

        [JsonPropertyName("billToCustomerNo")]
        public string? BillToCustomerNo { get; set; }

        [JsonPropertyName("roNo")]
        public string? RoNo { get; set; }

        [JsonPropertyName("rodate")]
        public string? RoDate { get; set; }

        [JsonPropertyName("salesperson")]
        public string? Salesperson { get; set; }

        [JsonPropertyName("locationcode")]
        public string? LocationCode { get; set; }

        [JsonPropertyName("invoiceDiscountAmountExclVat")]
        public decimal? InvoiceDiscountAmountExclVat { get; set; }

        [JsonPropertyName("invoiceDiscountPercent")]
        public decimal? InvoiceDiscountPercent { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonPropertyName("modifiedDateTime")]
        public DateTimeOffset? ModifiedDateTime { get; set; }

        [JsonPropertyName("salesLines")]
        public List<SalesOrderLineDto> SalesLines { get; set; } = new();
    }
}
