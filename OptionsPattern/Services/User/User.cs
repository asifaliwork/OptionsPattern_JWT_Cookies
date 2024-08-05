using Microsoft.AspNetCore.Identity;
using OptionsPattern.Data;
using OptionsPattern.Models.Account;

namespace OptionsPattern.Services.User
{
    public class User : IUser
    {
        private readonly ApplicationDbContext _db;
        SignInManager<ApplicationUser> _signInManager;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public User(ApplicationDbContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
        {
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public ApplicationUser GetUser()
        {   
            var user = _userManager.FindByEmailAsync(OptionsPattern.Utilities.Helper.Admin).GetAwaiter().GetResult();
           // var result = _signInManager.PasswordSignInAsync(user!, loginModel.Password!, false, true).GetAwaiter().GetResult();
            return user!;
        }
    }
}
