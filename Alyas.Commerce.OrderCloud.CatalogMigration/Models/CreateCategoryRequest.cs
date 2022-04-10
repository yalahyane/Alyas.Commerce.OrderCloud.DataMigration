namespace Alyas.Commerce.OrderCloud.CatalogMigration.Models
{
    using System.Security.RightsManagement;
    using Newtonsoft.Json;

    public class CreateCategoryRequest : BaseCatalogRequest
    {
        [JsonProperty("ParentID")]
        public string ParentId { get; set; }
    }
}
