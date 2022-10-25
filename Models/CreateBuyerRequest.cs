namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    using Newtonsoft.Json;

    public class CreateBuyerRequest : BaseCatalogRequest
    {
        [JsonProperty("DefaultCatalogID")]
        public string DefaultCatalogId { get; set; }
    }
}
