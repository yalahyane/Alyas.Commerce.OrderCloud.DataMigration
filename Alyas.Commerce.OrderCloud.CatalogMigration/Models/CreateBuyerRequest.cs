namespace Alyas.Commerce.OrderCloud.CatalogMigration.Models
{
    using Newtonsoft.Json;

    public class CreateBuyerRequest : BaseCatalogRequest
    {
        [JsonProperty("DefaultCatalogID")]
        public string DefaultCatalogId { get; set; }
    }
}
