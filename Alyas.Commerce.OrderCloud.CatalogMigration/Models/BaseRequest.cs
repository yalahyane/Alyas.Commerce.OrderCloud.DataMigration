namespace Alyas.Commerce.OrderCloud.CatalogMigration.Models
{
    using Newtonsoft.Json;

    public class BaseRequest
    {
        [JsonProperty("ID")]
        public string Id { get; set; }
    }
}
