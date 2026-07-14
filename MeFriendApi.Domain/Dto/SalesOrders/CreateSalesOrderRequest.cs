using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class CreateSalesOrderRequest
    {
        [JsonPropertyName("postingDate")]
        public string? PostingDate { get; set; }

        [JsonPropertyName("sellToCustomerNo")]
        public string? SellToCustomerNo { get; set; }

        [JsonPropertyName("clientNo")]
        public string? ClientNo { get; set; }

        [JsonPropertyName("clientName")]
        public string? ClientName { get; set; }

        [JsonPropertyName("clientAddress")]
        public string? ClientAddress { get; set; }

        [JsonPropertyName("clientAddress1")]
        public string? ClientAddress1 { get; set; }

        [JsonPropertyName("clientPhoneNo")]
        public string? ClientPhoneNo { get; set; }

        [JsonPropertyName("clientCity")]
        public string? ClientCity { get; set; }

        [JsonPropertyName("clientpostalcode")]
        public string? ClientPostalCode { get; set; }

        [JsonPropertyName("salesperson")]
        public string? Salesperson { get; set; }

        [JsonPropertyName("discountpercentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonPropertyName("salesLines")]
        public List<CreateSalesOrderLineRequest> SalesLines { get; set; } = new();
    }
}
