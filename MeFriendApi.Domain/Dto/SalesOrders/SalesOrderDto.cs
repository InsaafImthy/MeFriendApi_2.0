using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.SalesOrders
{
    public class SalesOrderDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        // The custom salesOrders API currently exposes the BC property as "number".
        // Keep the public API's established "no" property while accepting that wire name.
        [JsonPropertyName("number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BusinessCentralNumber
        {
            get => null;
            set
            {
                if (string.IsNullOrWhiteSpace(No))
                    No = value;
            }
        }

        [JsonPropertyName("sellToCustomerNo")]
        public string? SellToCustomerNo { get; set; }

        [JsonPropertyName("sellToCustomerName")]
        public string? SellToCustomerName { get; set; }

        [JsonPropertyName("clientNo")]
        public string? ClientNo { get; set; }

        [JsonPropertyName("clientName")]
        public string? ClientName { get; set; }

        [JsonPropertyName("postingDate")]
        public string? PostingDate { get; set; }

        [JsonPropertyName("orderDate")]
        public string? OrderDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("amountIncludingVAT")]
        public decimal? AmountIncludingVAT { get; set; }

        [JsonPropertyName("SalesOrderLines")]
        public List<SalesOrderLineDto> SalesOrderLines { get; set; } = new();

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
