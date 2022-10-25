namespace Alyas.Commerce.OrderCloud.DataMigration.Commands
{
    using System.Threading.Tasks;
    using Pipelines;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class MigrateCustomersCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public MigrateCustomersCommand(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public virtual async Task<CommerceCommand> Process(CommerceContext commerceContext)
        {
            await this._commerceCommander.Pipeline<IMigrateCustomersPipeline>().Run("", commerceContext.PipelineContext);
            return this;
        }
    }
}
