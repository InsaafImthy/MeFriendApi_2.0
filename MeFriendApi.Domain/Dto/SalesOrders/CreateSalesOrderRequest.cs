using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class CreateSalesOrderRequest
    {
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

        [JsonPropertyName("salesLines")]
        public List<CreateSalesOrderLineRequest> SalesLines { get; set; } = new();
    }
}
