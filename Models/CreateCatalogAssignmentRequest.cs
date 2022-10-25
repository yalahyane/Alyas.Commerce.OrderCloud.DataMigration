namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    using Newtonsoft.Json;

    public class CreateCatalogAssignmentRequest : BaseRequest
    {
        [JsonProperty("CatalogID")]
        public string CatalogId { get; set; }
        [JsonProperty("BuyerID")]
        public string BuyerId { get; set; }
        public bool ViewAllCategories { get; set; }
        public bool ViewAllProducts { get; set; }
    }
}
