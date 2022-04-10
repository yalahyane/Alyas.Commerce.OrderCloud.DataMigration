namespace Alyas.Commerce.OrderCloud.CatalogMigration.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public abstract class BaseOrderCloudBlock<TArg, TResult> :  PipelineBlock<TArg, TResult, CommercePipelineExecutionContext>
    {
        protected readonly CommerceCommander CommerceCommander;

        protected BaseOrderCloudBlock(CommerceCommander commander)
        {
            this.CommerceCommander = commander;
        }

        public virtual async Task<bool> MakeApiCall(BaseRequest request, CommercePipelineExecutionContext context, string url, string method = "POST", int numberOfTries = 0)
        {
            var client = new HttpClient();
            var policy = context.GetPolicy<OrderCloudPolicy>();
            var response = true;

            if (string.IsNullOrEmpty(policy.AccessToken))
            {
                await this.Authenticate(client, policy, context);
            }
            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = new HttpMethod(method);
                httpRequest.RequestUri = new Uri($"{policy.BaseUrl}{url}");
                httpRequest.Headers.Add("Authorization",$"Bearer {policy.AccessToken}");
                var json = JsonConvert.SerializeObject(request, SerializerSettings);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.SendAsync(httpRequest);
                if (result != null && result.IsSuccessStatusCode)
                { 
                    result.Dispose();
                }
                else if (numberOfTries <= 3 && result != null && result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    numberOfTries++;
                    policy.AccessToken = string.Empty;
                    await this.MakeApiCall(request, context, url, method, numberOfTries);
                }
                else
                {
                    response = false;
                }
                client.Dispose();
            }

            return response;
        }

        private JsonSerializerSettings _serializerSettings;
        private JsonSerializerSettings SerializerSettings
        {
            get
            {
                if (_serializerSettings != null) return _serializerSettings;
                lock (this)
                {
                    _serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    _serializerSettings.Converters.Add(new StringEnumConverter());
                }
                return _serializerSettings;
            }
        }

        private async Task Authenticate(HttpClient client, OrderCloudPolicy policy, CommercePipelineExecutionContext context)
        {
            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = new HttpMethod("POST");
                httpRequest.RequestUri = new Uri($"{policy.OauthUrl}");
                var authRequest = new []
                {
                    new KeyValuePair<string, string>("client_id", policy.ClientId),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", policy.UserName),
                    new KeyValuePair<string, string>("password", policy.Password),
                    new KeyValuePair<string, string>("client_secret", policy.ClientSecret),
                    new KeyValuePair<string, string>("scope", "CatalogAdmin BuyerReader MeAdmin InventoryAdmin PasswordReset OrderAdmin PriceScheduleAdmin ProductAdmin ProductAssignmentAdmin ShipmentAdmin")
                };
                httpRequest.Content = new FormUrlEncodedContent(authRequest);
                var result = await client.SendAsync(httpRequest);
                if (!result.IsSuccessStatusCode)
                {
                    context.Abort("Failed to Authenticate", context);
                    return;
                }

                var authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(await result.Content.ReadAsStringAsync());
                policy.AccessToken = authResponse.AccessToken;
                context.SetPolicy(policy);
            }
        }
    }
}
