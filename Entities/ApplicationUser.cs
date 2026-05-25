using Microsoft.AspNetCore.Identity;

namespace BoutiqueQuantity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
