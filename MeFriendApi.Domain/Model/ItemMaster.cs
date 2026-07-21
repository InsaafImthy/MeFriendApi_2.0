using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto
{
    public class ItemMaster
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("description2")]
        public string? Description2 { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("baseUnitOfMeasure")]
        public string? BaseUnitOfMeasure { get; set; }

        [JsonPropertyName("itemCategoryCode")]
        public string? ItemCategoryCode { get; set; }

        [JsonPropertyName("inventoryPostingGroup")]
        public string? InventoryPostingGroup { get; set; }

        [JsonPropertyName("genProdPostingGroup")]
        public string? GenProdPostingGroup { get; set; }

        [JsonPropertyName("unitCost")]
        public decimal? UnitCost { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
