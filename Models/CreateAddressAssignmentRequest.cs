namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    public class CreateAddressAssignmentRequest : BaseRequest
    {
        public string AddressId { get; set; }
        public string UserId { get; set; }
        public bool IsShipping { get; set; }
        public bool IsBilling { get; set; }
    }
}
