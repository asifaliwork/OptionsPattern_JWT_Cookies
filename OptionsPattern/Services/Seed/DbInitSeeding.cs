using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OptionsPattern.Data;
using OptionsPattern.Models.Account;

namespace OptionsPattern.Services.Seed
{
    public class DbInitSeeding : IDbInitSeeding
    {
        private readonly ApplicationDbContext db;
        public UserManager<ApplicationUser> userManager { get; }
        public RoleManager<IdentityRole> roleManager { get; }

        public DbInitSeeding(ApplicationDbContext _db, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            db = _db;
            roleManager = _roleManager;
            userManager = _userManager;
        }


        public void Initialize()
        {
            try
            {
                if (db.Database.GetPendingMigrations().Count() > 0)
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }
            if (db.Roles.Any(x => x.Name == Utilities.Helper.Admin)) return;
            else
            {
                roleManager.CreateAsync(new IdentityRole(Utilities.Helper.Admin)).GetAwaiter().GetResult();
            }

            userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                Name = "Admin"
            }, "Asd123@").GetAwaiter().GetResult();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Email == "admin@gmail.com");
            userManager.AddToRoleAsync(user, Utilities.Helper.Admin).GetAwaiter().GetResult();
        }
    }
}
