﻿//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using System.Threading.Tasks;

//namespace OptionsPattern.Middleware
//{
//    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
//    public class CookieMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public CookieMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public Task Invoke(HttpContext httpContext)
//        {

//            return _next(httpContext);
//        }
//    }

//    // Extension method used to add the middleware to the HTTP request pipeline.
//    public static class CookieMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseCookieMiddleware(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<CookieMiddleware>();
//        }
//    }
//}
