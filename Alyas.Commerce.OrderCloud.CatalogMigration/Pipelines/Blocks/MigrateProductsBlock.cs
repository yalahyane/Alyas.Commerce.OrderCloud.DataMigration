namespace Alyas.Commerce.OrderCloud.CatalogMigration.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;

    public class MigrateProductsBlock : BaseOrderCloudBlock<string, bool>
    {
        public MigrateProductsBlock(CommerceCommander commander) : base(commander)
        {
        }

        public override async Task<bool> Run(string arg, CommercePipelineExecutionContext context)
        {
            var sellableItems = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(SellableItem), CommerceEntity.ListName<SellableItem>(), 0, int.MaxValue), context).ConfigureAwait(false);
            var categories = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Category), CommerceEntity.ListName<Category>(), 0, int.MaxValue), context).ConfigureAwait(false);
            var catalogs = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Catalog), CommerceEntity.ListName<Catalog>(), 0, int.MaxValue), context).ConfigureAwait(false);

            if (sellableItems?.List.Items != null)
            {
                foreach (var commerceEntity in sellableItems.List.Items)
                {
                    var sellableItem = (SellableItem)commerceEntity;

                    await this.CreateProduct(sellableItem, context);
                    await this.CreatePriceSchedules(sellableItem, catalogs.List.Items, context);
                    
                    if (!string.IsNullOrEmpty(sellableItem.ParentCategoryList))
                    {
                        var parentCategories = categories.List.Items.Where(x =>
                            sellableItem.ParentCategoryList.Split('|').Contains(((Category)x).SitecoreId)).ToList();
                        if (parentCategories.Any())
                        {
                            foreach (var entity in parentCategories)
                            {
                                var category = (Category) entity;

                                var parentCatalog = catalogs.List.Items.FirstOrDefault(x =>
                                    category.ParentCatalogList.Split('|').Contains(((Catalog)x).SitecoreId));
                                if (parentCatalog != null)
                                {
                                    await this.CreateCategoryProductAssignment(sellableItem.Id,category.Id, parentCatalog.Id, context);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private async Task CreateProduct(SellableItem sellableItem, CommercePipelineExecutionContext context)
        {
            var request = new CreateProductRequest
            {
                Id = sellableItem.Id,
                Name = sellableItem.Name,
                Description = sellableItem.Description,
                Inventory = new Inventory
                {
                    Enabled = true,
                    VariantLevelTracking = true,
                    QuantityAvailable = context.GetPolicy<DefaultValuesPolicy>().DefaultInventoryQuantity
                }
            };
            await this.MakeApiCall(request, context, "products").ConfigureAwait(false);
        }

        private async Task CreateCategoryProductAssignment(string productId, string categoryId, string catalogId, CommercePipelineExecutionContext context)
        {
            var request = new CreateCategoryProductAssignmentRequest
            {
                CategoryId = categoryId,
                ProductId = productId
            };
            await this.MakeApiCall(request, context, $"catalogs/{catalogId}/categories/productassignments").ConfigureAwait(false);
        }

        private async Task CreatePriceSchedules(SellableItem sellableItem, List<CommerceEntity> catalogs, CommercePipelineExecutionContext context)
        {
            var priceSchedules =
                await this.GeneratePriceScheduleList(sellableItem, catalogs, context);
            foreach (var priceSchedule in priceSchedules)
            {
                await this.MakeApiCall(priceSchedule, context, "priceschedules").ConfigureAwait(false);
                var request = new CreateProductAssignmentRequest
                {
                    ProductId = sellableItem.Id,
                    PriceScheduleId = priceSchedule.Id,
                    BuyerId = context.GetPolicy<DefaultValuesPolicy>().DefaultBuyerId
                };
                await this.MakeApiCall(request, context, "product/assignments").ConfigureAwait(false);
            }
        }
        private async Task<List<CreatePriceScheduleRequest>> GeneratePriceScheduleList(SellableItem sellableItem, List<CommerceEntity> catalogs, CommercePipelineExecutionContext context)
        {
            var priceSchedules = new List<CreatePriceScheduleRequest>();
            foreach (var listPrice in sellableItem.GetPolicy<ListPricingPolicy>().Prices)
            {
                var priceSchedule = new CreatePriceScheduleRequest
                {
                    Id = $"{sellableItem.ProductId}_{listPrice.CurrencyCode}",
                    Name = $"{sellableItem.ProductId}_{listPrice.CurrencyCode}",
                    Currency = listPrice.CurrencyCode
                };
                priceSchedule.PriceBreaks.Add(new PriceBreak { Quantity = 1, Price = listPrice.Amount });
                priceSchedules.Add(priceSchedule);
            }
            if (sellableItem.HasPolicy<PriceCardPolicy>() && !string.IsNullOrEmpty(sellableItem.GetPolicy<PriceCardPolicy>().PriceCardName))
            {
                foreach (var catalogEntity in catalogs)
                {
                    var catalog = (Catalog)catalogEntity;
                    var priceCardId = $"{catalog.PriceBookName}-{sellableItem.GetPolicy<PriceCardPolicy>().PriceCardName}";
                    if (await this.CommerceCommander.Pipeline<IFindEntityPipeline>()
                        .Run(new FindEntityArgument(typeof(PriceCard), priceCardId), context) is PriceCard priceCard)
                    {
                        foreach (var snapshot in priceCard.Snapshots)
                        {
                            foreach (var tier in snapshot.Tiers)
                            {
                                var listPrice = sellableItem.GetPolicy<ListPricingPolicy>().Prices
                                    .FirstOrDefault(x =>
                                        x.CurrencyCode.Equals(tier.Currency,
                                            StringComparison.OrdinalIgnoreCase));
                                if (listPrice != null)
                                {
                                    var priceSchedule =
                                        priceSchedules.FirstOrDefault(x => x.Currency.Equals(tier.Currency));
                                    if (priceSchedule != null)
                                    {
                                        var existingPriceBreak =
                                            priceSchedule.PriceBreaks.FirstOrDefault(x =>
                                                x.Quantity == tier.Quantity);
                                        if (existingPriceBreak != null)
                                        {
                                            existingPriceBreak.SalePrice = tier.Price;
                                        }
                                        else
                                        {
                                            priceSchedule.PriceBreaks.Add(new PriceBreak
                                            {
                                                Quantity = (int)tier.Quantity,
                                                Price = listPrice.Amount * tier.Quantity,
                                                SalePrice = tier.Price
                                            });
                                        }

                                        priceSchedule.SaleStart = snapshot.BeginDate.DateTime;
                                    }
                                }

                            }
                        }
                    }
                }

            }

            return priceSchedules;
        }
    }
}
