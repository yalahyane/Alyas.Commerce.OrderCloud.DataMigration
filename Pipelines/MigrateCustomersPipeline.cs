namespace Alyas.Commerce.OrderCloud.DataMigration.Pipelines
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public class MigrateCustomersPipeline : CommercePipeline<string, string>, IMigrateCustomersPipeline
    {
        public MigrateCustomersPipeline(IPipelineConfiguration<IMigrateCustomersPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
