using OptionsPattern.Models.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OptionsPattern.Services.Account
{
    public interface IAccountServices
    {
        public LoginResponse Login(LoginModel loginModel);

        public LoginResponse SecondLogin(LoginModel loginModel);
        public LoginResponse CookieLogin(LoginModel loginModel);
        public RegisterResponse Register(RegisterModel registerModel);

        public JwtSecurityToken GetToken(List<Claim> authClaims);

        public Task<string> logout();
    }
}
