using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptionsPattern.Models.Account;
using OptionsPattern.Services.Account;
using System.Security.Claims;


namespace OptionsPattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;

        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [Authorize]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return Ok("Every thing  Is Okay");
        }

        [HttpPost("login")]
        public IActionResult login(LoginModel loginModel)
        {
            var result = _accountServices.Login(loginModel);
        
            return Ok(result);
        }
        [HttpPost("SecondLogin")]
        public IActionResult SecondLogin(LoginModel loginModel)
        {
            var result = _accountServices.SecondLogin(loginModel);
            return Ok(result);
        }
        [HttpPost("CookieLogin")]
        public IActionResult CookieLogin(LoginModel loginModel)
        {
            var result = _accountServices.CookieLogin(loginModel);
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Email, loginModel.Email!)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).GetAwaiter().GetResult();
            return Ok(result);
        }


        [HttpPost("Register")]
        public IActionResult Register(RegisterModel registerModel)
        {
            var result = _accountServices.Register(registerModel);
            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult logout()
        {
            var result = _accountServices.logout();
            return Ok(result);
        }
    }
}
