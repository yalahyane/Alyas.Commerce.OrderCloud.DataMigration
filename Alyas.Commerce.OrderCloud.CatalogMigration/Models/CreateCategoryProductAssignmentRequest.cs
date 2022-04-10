namespace Alyas.Commerce.OrderCloud.CatalogMigration.Models
{
    using Newtonsoft.Json;

    public class CreateCategoryProductAssignmentRequest : BaseRequest
    {
        [JsonProperty("CategoryID")]
        public string CategoryId { get; set; }
        [JsonProperty("ProductID")]
        public string ProductId { get; set; }
    }
}
