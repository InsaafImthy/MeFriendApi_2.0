using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.Salespersons
{
    public class SalespersonDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("mdmCode")]
        public string? MdmCode { get; set; }
    }
}
