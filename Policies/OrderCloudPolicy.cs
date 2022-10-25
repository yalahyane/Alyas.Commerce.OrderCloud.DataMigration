namespace Alyas.Commerce.OrderCloud.DataMigration.Policies
{
    using Sitecore.Commerce.Core;

    public class OrderCloudPolicy : Policy
    {
        public string BaseUrl { get; set; } = "https://sandboxapi.ordercloud.io/v1/";
        public string OauthUrl { get; set; } = "https://sandboxapi.ordercloud.io/oauth/token";
        public string ClientId { get; set; } = "[Your API Client Id]";
        public string ClientSecret { get; set; } = "[Your API Client Secret]";
        public string AccessToken { get; set; }
    }
}
