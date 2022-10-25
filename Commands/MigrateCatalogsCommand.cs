namespace Alyas.Commerce.OrderCloud.DataMigration.Commands
{
    using System.Threading.Tasks;
    using Pipelines;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class MigrateCatalogsCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public MigrateCatalogsCommand(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public virtual async Task<CommerceCommand> Process(CommerceContext commerceContext)
        {
            await this._commerceCommander.Pipeline<IMigrateCatalogsPipeline>().Run("", commerceContext.PipelineContext);
            return this;
        }
    }
}
