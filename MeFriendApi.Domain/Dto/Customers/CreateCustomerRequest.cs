using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto
{
    public class CreateCustomerRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("name2")]
        public string? Name2 { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("address2")]
        public string? Address2 { get; set; }

        [JsonPropertyName("stateCode")]
        public string? StateCode { get; set; }

        [JsonPropertyName("countryRegionCode")]
        public string? CountryRegionCode { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("postCode")]
        public string? PostCode { get; set; }

        [JsonPropertyName("locationCode")]
        public string? LocationCode { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("PAN")]
        public string? PAN { get; set; }

        [JsonPropertyName("gstRegistrationNo")]
        public string? GstRegistrationNo { get; set; }

        [JsonPropertyName("genPostingGroup")]
        public string? GenPostingGroup { get; set; }

        [JsonPropertyName("customerPostingGroup")]
        public string? CustomerPostingGroup { get; set; }

        [JsonPropertyName("gstCustomerType")]
        public string? GstCustomerType { get; set; }
    }
}
