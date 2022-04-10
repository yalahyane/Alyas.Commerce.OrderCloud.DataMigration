namespace Alyas.Commerce.OrderCloud.CatalogMigration.Models
{
    public class BaseCatalogRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;
    }
}
