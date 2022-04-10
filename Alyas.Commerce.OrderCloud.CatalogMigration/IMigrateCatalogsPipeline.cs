namespace Alyas.Commerce.OrderCloud.CatalogMigration
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public interface IMigrateCatalogsPipeline : IPipeline<string, string, CommercePipelineExecutionContext>
    {
    }
}
