using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OptionsPattern.Data;
using OptionsPattern.Models.Account;
using OptionsPattern.Models.AppSettings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace OptionsPattern.Services.Account
{
    public class AccountServices : IAccountServices
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _db;
        SignInManager<ApplicationUser> _signInManager;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;


        public AccountServices(ApplicationDbContext db,
        SignInManager<ApplicationUser> signInManager,
        IOptions<AppSettings> appSettings,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
        
        {
            _db = db;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            //this.http = http;
        }

        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            var token = new JwtSecurityToken
                (

                 issuer: _configuration["JWT:ValidIssuer"],

                 audience: _configuration["JWT:ValidAudience"],

                 expires: DateTime.Now.AddSeconds(110),

                 claims: authClaims,

                 signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        public  LoginResponse Login(LoginModel loginModel)
        {
            var responseModel = new LoginResponse();

            try

            {
                var user =  _userManager.FindByEmailAsync(loginModel.Email!).GetAwaiter().GetResult();
                if (user != null)
                {
                    var result =  _signInManager.PasswordSignInAsync(user, loginModel.Password!, false , true).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                        new Claim("Email" , user.Email!.ToString()),
                    };
                        var token = GetToken(authClaims);
                        responseModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
                        responseModel.Status = "Success";
                        responseModel.Message = "Login Success";
                    }
                     _userManager.UpdateAsync(user).GetAwaiter().GetResult();
                    if (result.IsLockedOut)
                    {
                        responseModel.Status = "Error";
                        responseModel.Message = "Your Account is locked out";
                    }
                }

            }
            catch (Exception ex)
            {
                return new LoginResponse { Message = ex.Message };
            }
            return responseModel;

        }

        public LoginResponse SecondLogin(LoginModel loginModel)
        {
            var responseModel = new LoginResponse();
            try
            {
                var user = _userManager.FindByEmailAsync(loginModel.Email!).GetAwaiter().GetResult();
                if (user != null)
                {
                    var result = _signInManager.PasswordSignInAsync(user, loginModel.Password!, false, true).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {  
                        var token = generateToken(user);
                        responseModel.Token = token;
                        responseModel.Status = "Success";
                        responseModel.Message = "Login Success";
                    }
                    _userManager.UpdateAsync(user).GetAwaiter().GetResult();
                    if (result.IsLockedOut)
                    {
                        responseModel.Status = "Error";
                        responseModel.Message = "Your Account is locked out";
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Message = ex.Message };
            }
            return responseModel;
        }

        private string generateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Key!));
            var credetial = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>()
            {
               new Claim("Id",Convert.ToString(user.Id)),
         
               new Claim(JwtRegisteredClaimNames.Email, user.Email!),
               
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in user.Name!)
            {
                claims.Add(new Claim("Role", Convert.ToString(role)));
            }
            var token = new JwtSecurityToken(_appSettings.ValidIssuer, _appSettings.ValidAudience, claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: credetial);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public LoginResponse CookieLogin(LoginModel loginModel)
        {
            var responseModel = new LoginResponse();
            try
            {
                var user = _userManager.FindByEmailAsync(loginModel.Email!).GetAwaiter().GetResult();
                if (user != null)
                {
                    var result = _signInManager.PasswordSignInAsync(user, loginModel.Password!, false, true).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        responseModel.Status = "Success";
                        responseModel.Message = "Login Success";
                    }
                    _userManager.UpdateAsync(user).GetAwaiter().GetResult();
                    if (result.IsLockedOut)
                    {
                        responseModel.Status = "Error";
                        responseModel.Message = "Your Account is locked out";
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Message = ex.Message };
            }
            return responseModel;
        }

        public async Task<string> logout()
        {
            await _signInManager.SignOutAsync();
            return "Your Account is Locked Try Again After 5 Mints";
        }

        public RegisterResponse Register(RegisterModel registerModel)
        {
            try
            {
                var registerResponse = new RegisterResponse();
                var user = new ApplicationUser
                {
                    UserName = registerModel.EmailAddress,
                    Email = registerModel.EmailAddress,
                    NormalizedUserName = registerModel.EmailAddress!.ToUpper(),
                    NormalizedEmail = registerModel.EmailAddress.ToUpper(),
                    Name = registerModel.RoleName,
                };

                var result =  _userManager.CreateAsync(user, registerModel.Password!).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    var role =  _roleManager.FindByNameAsync("User").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user, registerModel.RoleName!).GetAwaiter().GetResult();
                    _signInManager.SignInAsync(user, isPersistent: false).GetAwaiter().GetResult();
                    return new RegisterResponse { Message = "Register Done"} ;
                }
                registerResponse.Message = "Try Again";
                return registerResponse;
            }
            catch (Exception ex)
            {
                return new RegisterResponse { Message = ex.Message };
            }
        }
    }
}
 