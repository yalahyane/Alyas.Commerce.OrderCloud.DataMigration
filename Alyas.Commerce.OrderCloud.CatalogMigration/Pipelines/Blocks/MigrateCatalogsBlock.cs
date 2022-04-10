namespace Alyas.Commerce.OrderCloud.CatalogMigration.Pipelines.Blocks
{
    using System.Threading.Tasks;
    using Models;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;

    public class MigrateCatalogsBlock : BaseOrderCloudBlock<string, string>
    {
        public MigrateCatalogsBlock(CommerceCommander commander): base(commander)
        {

        }
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var catalogs = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Catalog), CommerceEntity.ListName<Catalog>(), 0, int.MaxValue), context).ConfigureAwait(false);
            if (catalogs?.List.Items != null)
            {
                await this.CreateBuyer(context).ConfigureAwait(false);
                foreach (var commerceEntity in catalogs.List.Items)
                {
                    var catalog = (Catalog) commerceEntity;
                    await this.CreateCatalog(catalog, context).ConfigureAwait(false);
                    await this.CreateCatalogAssignment(catalog, context).ConfigureAwait(false);
                }
            }

            return arg;
        }

        public async Task CreateCatalog(Catalog catalog, CommercePipelineExecutionContext context)
        {
            var request = new CreateCatalogRequest
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Description = catalog.DisplayName
            };
            await this.MakeApiCall(request, context, "catalogs").ConfigureAwait(false);
        }

        public async Task CreateBuyer(CommercePipelineExecutionContext context)
        {
            var defaultPolicy = context.GetPolicy<DefaultValuesPolicy>();
            var request = new CreateBuyerRequest
            {
                Id = defaultPolicy.DefaultBuyerId,
                Name = defaultPolicy.DefaultBuyerId,
                DefaultCatalogId = context.GetPolicy<DefaultValuesPolicy>().DefaultCatalogId
            };
            await this.MakeApiCall(request, context, "buyers").ConfigureAwait(false);
        }

        public async Task CreateCatalogAssignment(Catalog catalog, CommercePipelineExecutionContext context)
        {
            var request = new CreateCatalogAssignmentRequest
            {
                CatalogId = catalog.Id,
                BuyerId = context.GetPolicy<DefaultValuesPolicy>().DefaultBuyerId,
                ViewAllCategories = true,
                ViewAllProducts = true
            };
            await this.MakeApiCall(request, context, "catalogs/assignments").ConfigureAwait(false);
        }
    }
}
