namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    using Newtonsoft.Json;

    public class BaseRequest
    {
        [JsonProperty("ID")]
        public string Id { get; set; }
    }
}
