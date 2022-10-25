﻿namespace Alyas.Commerce.OrderCloud.DataMigration.Pipelines
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public class MigrateCatalogsPipeline : CommercePipeline<string, string>, IMigrateCatalogsPipeline
    {
        public MigrateCatalogsPipeline(IPipelineConfiguration<IMigrateCatalogsPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
