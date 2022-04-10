namespace Alyas.Commerce.OrderCloud.CatalogMigration.Pipelines.Blocks
{
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;

    public class MigrateCategoriesBlock : BaseOrderCloudBlock<string, string>
    {
        public MigrateCategoriesBlock(CommerceCommander commander): base(commander)
        {

        }
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var catalogs = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Catalog), CommerceEntity.ListName<Catalog>(), 0, int.MaxValue), context).ConfigureAwait(false);
            var categories = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Category), CommerceEntity.ListName<Category>(), 0, int.MaxValue), context).ConfigureAwait(false);
            if (categories?.List.Items != null)
            {
                await this.CreateCategories(catalogs, categories, context);
                await this.UpdateParentCategoryIds(catalogs, categories, context);
            }

            return arg;
        }

        private async Task CreateCategories(FindEntitiesInListArgument catalogs, FindEntitiesInListArgument categories, CommercePipelineExecutionContext context)
        {
            foreach (var commerceEntity in categories.List.Items)
            {
                var category = (Category)commerceEntity;

                var request = new CreateCategoryRequest
                {
                    Id = category.Id.Replace(" ", ""),
                    Name = category.Name,
                    Description = category.DisplayName
                };

                if (!string.IsNullOrEmpty(category.ParentCatalogList))
                {
                    var parentCatalog = catalogs.List.Items.FirstOrDefault(x =>
                        category.ParentCatalogList.Split('|').Contains(((Catalog)x).SitecoreId));
                    if (parentCatalog != null)
                    {
                        await this.MakeApiCall(request, context, $"catalogs/{parentCatalog.Id}/categories");
                    }
                }
            }
        }

        private async Task UpdateParentCategoryIds(FindEntitiesInListArgument catalogs, FindEntitiesInListArgument categories, CommercePipelineExecutionContext context)
        {
            foreach (var commerceEntity in categories.List.Items)
            {
                var category = (Category)commerceEntity;
                
                if (!string.IsNullOrEmpty(category.ParentCategoryList))
                {
                    var parentCategories = categories.List.Items.Where(x =>
                        category.ParentCategoryList.Split('|').Contains(((Category)x).SitecoreId)).ToList();
                    if (parentCategories.Any())
                    {
                        var request = new CreateCategoryRequest
                        {
                            ParentId= parentCategories.First().Id.Replace(" ", "")
                        };
                        if (!string.IsNullOrEmpty(category.ParentCatalogList))
                        {
                            var parentCatalog = catalogs.List.Items.FirstOrDefault(x =>
                                category.ParentCatalogList.Split('|').Contains(((Catalog)x).SitecoreId));
                            if (parentCatalog != null)
                            {
                                await this.MakeApiCall(request, context, $"catalogs/{parentCatalog.Id}/categories/{category.Id.Replace(" ","")}", "PATCH");
                            }
                        }
                    }
                }
            }
        }
    }
}
