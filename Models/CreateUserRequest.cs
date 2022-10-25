namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    public class CreateUserRequest : BaseRequest
    {
        public string Username { get; set; }    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; } = true;
    }
}
