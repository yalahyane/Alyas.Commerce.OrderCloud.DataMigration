namespace Alyas.Commerce.OrderCloud.CatalogMigration.Policies
{
    using Sitecore.Commerce.Core;

    public class OrderCloudPolicy : Policy
    {
        public string BaseUrl { get; set; } = "https://sandboxapi.ordercloud.io/v1/";
        public string OauthUrl { get; set; } = "https://sandboxapi.ordercloud.io/oauth/token";
        public string ClientId { get; set; } = "[Your Client Id]";
        public string ClientSecret { get; set; } = "[Your Client Secret]";
        public string UserName { get; set; } = "[Your username]";
        public string Password { get; set; } = "[Your Password]";
        public string AccessToken { get; set; }

    }
}
