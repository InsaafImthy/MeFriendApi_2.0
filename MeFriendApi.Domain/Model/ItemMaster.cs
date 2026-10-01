using System.Text.Json.Serialization;

namespace MeFriendApi.Domain.Dto
{
    public class ItemMaster
    {
        [JsonPropertyName("@odata.etag")]
        public string? ODataEtag { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("description2")]
        public string? Description2 { get; set; }

        [JsonPropertyName("searchDescription")]
        public string? SearchDescription { get; set; }

        [JsonPropertyName("baseUnitOfMeasureCode")]
        public string? BaseUnitOfMeasureCode { get; set; }

        [JsonPropertyName("itemCategoryCode")]
        public string? ItemCategoryCode { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("inventoryPostingGroupCode")]
        public string? InventoryPostingGroupCode { get; set; }

        [JsonPropertyName("generalProductPostingGroupCode")]
        public string? GeneralProductPostingGroupCode { get; set; }

        [JsonPropertyName("vatProductPostingGroupCode")]
        public string? VatProductPostingGroupCode { get; set; }

        [JsonPropertyName("costingMethod")]
        public string? CostingMethod { get; set; }

        [JsonPropertyName("unitCost")]
        public decimal? UnitCost { get; set; }

        [JsonPropertyName("standardCost")]
        public decimal? StandardCost { get; set; }

        [JsonPropertyName("lastDirectCost")]
        public decimal? LastDirectCost { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("profitPercentage")]
        public decimal? ProfitPercentage { get; set; }

        [JsonPropertyName("vendorNumber")]
        public string? VendorNumber { get; set; }

        [JsonPropertyName("vendorItemNumber")]
        public string? VendorItemNumber { get; set; }

        [JsonPropertyName("leadTimeCalculation")]
        public string? LeadTimeCalculation { get; set; }

        [JsonPropertyName("reorderingPolicy")]
        public string? ReorderingPolicy { get; set; }

        [JsonPropertyName("reorderPoint")]
        public decimal? ReorderPoint { get; set; }

        [JsonPropertyName("reorderQuantity")]
        public decimal? ReorderQuantity { get; set; }

        [JsonPropertyName("safetyStockQuantity")]
        public decimal? SafetyStockQuantity { get; set; }

        [JsonPropertyName("maximumInventory")]
        public decimal? MaximumInventory { get; set; }

        [JsonPropertyName("minimumOrderQuantity")]
        public decimal? MinimumOrderQuantity { get; set; }

        [JsonPropertyName("maximumOrderQuantity")]
        public decimal? MaximumOrderQuantity { get; set; }

        [JsonPropertyName("orderMultiple")]
        public decimal? OrderMultiple { get; set; }

        [JsonPropertyName("grossWeight")]
        public decimal? GrossWeight { get; set; }

        [JsonPropertyName("netWeight")]
        public decimal? NetWeight { get; set; }

        [JsonPropertyName("unitVolume")]
        public decimal? UnitVolume { get; set; }

        [JsonPropertyName("shelfNumber")]
        public string? ShelfNumber { get; set; }

        [JsonPropertyName("countryRegionOfOriginCode")]
        public string? CountryRegionOfOriginCode { get; set; }

        [JsonPropertyName("tariffNumber")]
        public string? TariffNumber { get; set; }

        [JsonPropertyName("blocked")]
        public bool? Blocked { get; set; }

        [JsonPropertyName("salesBlocked")]
        public bool? SalesBlocked { get; set; }

        [JsonPropertyName("purchasingBlocked")]
        public bool? PurchasingBlocked { get; set; }

        [JsonPropertyName("lastDateModified")]
        public string? LastDateModified { get; set; }

        [JsonPropertyName("globalDimension1Code")]
        public string? GlobalDimension1Code { get; set; }

        [JsonPropertyName("globalDimension2Code")]
        public string? GlobalDimension2Code { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonPropertyName("modifiedDateTime")]
        public DateTimeOffset? ModifiedDateTime { get; set; }
    }
}
