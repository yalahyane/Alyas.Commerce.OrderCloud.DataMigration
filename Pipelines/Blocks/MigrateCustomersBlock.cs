namespace Alyas.Commerce.OrderCloud.DataMigration.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Customers;

    public class MigrateCustomersBlock : BaseOrderCloudBlock<string, string>
    {
        public MigrateCustomersBlock(CommerceCommander commander) : base(commander)
        {
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var findEntitiesResult = await this.CommerceCommander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Customer), CommerceEntity.ListName<Customer>(), 0, int.MaxValue) {LoadEntities = false}, context).ConfigureAwait(false);
            var defaultPolicy = context.GetPolicy<DefaultValuesPolicy>();

            foreach (var entityReference in findEntitiesResult.EntityReferences)
            {
                if (await this.CommerceCommander.Pipeline<IFindEntityPipeline>().Run(new FindEntityArgument(typeof(Customer), entityReference.EntityId), context).ConfigureAwait(false) is Customer customer)
                {
                    var detailsEntityView = customer.GetComponent<CustomerDetailsComponent>()?.View.ChildViews
                        .FirstOrDefault(v => v.Name.Equals(context.GetPolicy<KnownCustomerViewsPolicy>().Details,
                            StringComparison.OrdinalIgnoreCase)) as EntityView;

                    var request = new CreateUserRequest
                    {
                        Id = customer.Id,
                        Username =customer.Email,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email =customer.Email,
                        Phone = detailsEntityView?.Properties.FirstOrDefault(p=>p.Name.Equals("PhoneNumber", StringComparison.OrdinalIgnoreCase))?.Value
                    };

                    await this.MakeApiCall(request, context, $"buyers/{defaultPolicy.DefaultBuyerId}/users").ConfigureAwait(false);

                    foreach (var addressComponent in customer.EntityComponents.OfType<AddressComponent>())
                    {
                        var addressRequest = new CreateAddressRequest
                        {
                            Id = addressComponent.Id,
                            AddressName = addressComponent.Party?.AddressName,
                            FirstName = addressComponent.Party?.FirstName,
                            LastName = addressComponent.Party?.LastName,
                            Street1 = addressComponent.Party?.Address1,
                            Street2 = addressComponent.Party?.Address2,
                            City = addressComponent.Party?.City,
                            State = addressComponent.Party?.StateCode ?? addressComponent.Party?.State,
                            Zip = addressComponent.Party?.ZipPostalCode,
                            Country = addressComponent.Party?.CountryCode ?? addressComponent.Party?.Country,
                            Phone = addressComponent.Party?.PhoneNumber
                        };

                        await this.MakeApiCall(addressRequest, context, $"buyers/{defaultPolicy.DefaultBuyerId}/addresses").ConfigureAwait(false);

                        var addressAssignmentRequest = new CreateAddressAssignmentRequest
                        {
                            AddressId = addressComponent.Id,
                            UserId = customer.Id,
                            IsShipping = addressComponent.Party.IsPrimary,
                            IsBilling = addressComponent.Party.IsPrimary
                        };
                        await this.MakeApiCall(addressAssignmentRequest, context, $"buyers/{defaultPolicy.DefaultBuyerId}/addresses/assignments").ConfigureAwait(false);
                    }
                }
            }

            return arg;
        }
    }
}
