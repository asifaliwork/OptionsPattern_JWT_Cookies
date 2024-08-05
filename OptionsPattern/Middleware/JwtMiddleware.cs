//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Text;
//using System.Threading.Tasks;


////using Custom_Jwt_Token_Example.Services;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Text;
//using OptionsPattern.Models.AppSettings;
//using Microsoft.AspNetCore.Identity;
//using OptionsPattern.Models.Account;
//using OptionsPattern.Services.User;

////namespace OptionsPattern.Middleware
////{
////    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
////    public class JwtMiddleware
////    {
////        private readonly RequestDelegate _next;

////        public JwtMiddleware(RequestDelegate next)
////        {
////            _next = next;
////        }

////        public Task Invoke(HttpContext httpContext)
////        {
////            return _next(httpContext);
////        }
////    }

////    // Extension method used to add the middleware to the HTTP request pipeline.
////    public static class JwtMiddlewareExtensions
////    {
////        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
////        {
////            return builder.UseMiddleware<JwtMiddleware>();
////        }
////    }
////}







//namespace OptionsPattern.Middleware
//{
//    public class JwtMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly AppSettings _appSettings;
//        private readonly IUser _user;
//       // UserManager<ApplicationUser> _userManager;
//        public JwtMiddleware(RequestDelegate _next, IOptions<AppSettings> _appSettings)
//        {
//            this._next = _next;
//            this._appSettings = _appSettings.Value;
//            //this._user = user;

//        }
//        public async Task Invoke(HttpContext context, IUser user)
//        {

//            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//            if (token != null)
//                //Validate Token
//                attachUserToContext(context, user, token);
//            _next(context);
//        }

//        private void attachUserToContext(HttpContext context, IUser user, string token)
//        {
//            try
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Key!));
//                tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateAudience = true,
                    
//                    ValidateIssuer = true,
//                    ValidateActor = false,
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = key,
//                    ClockSkew = TimeSpan.Zero,
//                    ValidateLifetime = true,
//                    ValidAudience = _appSettings.ValidAudience,
//                    ValidIssuer = _appSettings.ValidIssuer,
//                }, out SecurityToken validateToken);
//                var jwtToken = (JwtSecurityToken)validateToken;
//               // var userId = int.Parse(jwtToken.Claims.FirstOrDefault(_ => _.Type == "Id").Value);
//                context.Items["Email"] = user.GetUser();

//            }
//            catch (Exception ex)
//            {


//            }
//        }
//    }
//}
