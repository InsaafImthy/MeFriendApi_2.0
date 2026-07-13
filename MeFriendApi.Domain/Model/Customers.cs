using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto
{
    public class Customers
    {
        [JsonPropertyName("@odata.etag")]
        public string ODataEtag { get; set; } = null!;

        [JsonPropertyName("no")]
        public string No { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("name2")]
        public string Name2 { get; set; } = null!;

        [JsonPropertyName("address")]
        public string Address { get; set; } = null!;

        [JsonPropertyName("stateCode")]
        public string StateCode { get; set; } = null!;

        [JsonPropertyName("countryRegionCode")]
        public string CountryRegionCode { get; set; } = null!;

        [JsonPropertyName("city")]
        public string City { get; set; } = null!;

        [JsonPropertyName("postCode")]
        public string PostCode { get; set; } = null!;

        [JsonPropertyName("locationCode")]
        public string LocationCode { get; set; } = null!;

        [JsonPropertyName("PAN")]
        public string PAN { get; set; } = null!;

        [JsonPropertyName("gstRegistrationNo")]
        public string GstRegistrationNo { get; set; } = null!;

        [JsonPropertyName("genPostingGroup")]
        public string GenPostingGroup { get; set; } = null!;

        [JsonPropertyName("customerPostingGroup")]
        public string CustomerPostingGroup { get; set; } = null!;

        [JsonPropertyName("gstCustomerType")]
        public string GstCustomerType { get; set; } = null!;
    }
}
