namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    using Newtonsoft.Json;

    public class CreateCategoryRequest : BaseCatalogRequest
    {
        [JsonProperty("ParentID")]
        public string ParentId { get; set; }
    }
}
