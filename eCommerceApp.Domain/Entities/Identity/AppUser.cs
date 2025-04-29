using Microsoft.AspNetCore.Identity;

namespace eCommerceApp.Domain.Entities.Identity
{
    public class AppUser : IdentityUser //Creating the User Object, using the Identity user properties to be consumed
    {
        public string FullName { get; set; } = string.Empty;
    }
}
