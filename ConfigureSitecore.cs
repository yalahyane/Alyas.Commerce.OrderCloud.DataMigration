namespace Alyas.Commerce.OrderCloud.DataMigration
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Pipelines;
    using Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {

            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);


            services.Sitecore().Pipelines(config => config
                .AddPipeline<IMigrateCatalogsPipeline, MigrateCatalogsPipeline>(configure =>
                {
                    configure.Add<MigrateCatalogsBlock>();
                    configure.Add<MigrateCategoriesBlock>();
                    configure.Add<MigrateProductsBlock>();
                })
                .AddPipeline<IMigrateCustomersPipeline, MigrateCustomersPipeline>(configure => configure.Add<MigrateCustomersBlock>())
                .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}
