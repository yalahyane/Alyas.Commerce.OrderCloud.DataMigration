namespace Alyas.Commerce.OrderCloud.DataMigration.Controllers
{
    using System;
    using System.Web.Http.OData;
    using Commands;
    using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;

    public class CommandsController : CommerceController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [Route("MigrateCatalogsToOrderCloud()")]
        public IActionResult MigrateCatalogsToOrderCloud([FromBody] ODataActionParameters value)
        {
            if (!this.ModelState.IsValid)
                return new BadRequestObjectResult(this.ModelState);
            var command = this.Command<MigrateCatalogsCommand>();
            return new ObjectResult(ExecuteLongRunningCommand(() => command.Process(this.CurrentContext)));
        }

        [HttpPut]
        [Route("MigrateCustomersToOrderCloud()")]
        public IActionResult MigrateCustomersToOrderCloud([FromBody] ODataActionParameters value)
        {
            if (!this.ModelState.IsValid)
                return new BadRequestObjectResult(this.ModelState);
            var command = this.Command<MigrateCustomersCommand>();
            return new ObjectResult(ExecuteLongRunningCommand(() => command.Process(this.CurrentContext)));
        }
    }
}
