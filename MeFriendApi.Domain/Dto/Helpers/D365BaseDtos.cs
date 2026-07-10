using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto.Helpers
{
    public class BcODataResponse<T>
    {
        public List<T> Value { get; set; } = new();
    }

    public class CompanyDto
    {
        [JsonPropertyName("@odata.etag")]
        public string? OdataEtag { get; set; }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("systemVersion")]
        public string SystemVersion { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("businessProfileId")]
        public string BusinessProfileId { get; set; } = string.Empty;

        [JsonPropertyName("systemCreatedAt")]
        public DateTime SystemCreatedAt { get; set; }

        [JsonPropertyName("systemCreatedBy")]
        public Guid SystemCreatedBy { get; set; }

        [JsonPropertyName("systemModifiedAt")]
        public DateTime SystemModifiedAt { get; set; }

        [JsonPropertyName("systemModifiedBy")]
        public Guid SystemModifiedBy { get; set; }
    }
}
