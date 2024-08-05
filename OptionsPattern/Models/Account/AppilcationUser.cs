using Microsoft.AspNetCore.Identity;

namespace OptionsPattern.Models.Account
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
