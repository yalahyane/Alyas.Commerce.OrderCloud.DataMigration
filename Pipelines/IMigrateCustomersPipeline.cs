namespace Alyas.Commerce.OrderCloud.DataMigration.Pipelines
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public interface IMigrateCustomersPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}
