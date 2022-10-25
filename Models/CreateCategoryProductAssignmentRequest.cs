namespace Alyas.Commerce.OrderCloud.DataMigration.Models
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
