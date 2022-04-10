namespace Alyas.Commerce.OrderCloud.CatalogMigration.Policies
{
    using Sitecore.Commerce.Core;

    public class DefaultValuesPolicy : Policy
    {
        public int DefaultInventoryQuantity { get; set; } = 1000;
        public string DefaultCatalogId { get; set; } = "[Your Default Catalog Id]";
        public string DefaultBuyerId { get; set; } = "[Your default Buyer Id]";
    }
}
