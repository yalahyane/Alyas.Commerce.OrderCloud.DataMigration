namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    using Newtonsoft.Json;

    public class CreateProductAssignmentRequest : BaseRequest
    {
        [JsonProperty("ProductID")]
        public string ProductId { get; set; }
        [JsonProperty("BuyerID")]
        public string BuyerId { get; set; }
        [JsonProperty("PriceScheduleID")]
        public string PriceScheduleId { get; set; }
    }
}
