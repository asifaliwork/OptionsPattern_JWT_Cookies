﻿//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using System.Threading.Tasks;

//namespace OptionsPattern.Middleware
//{
//    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
//    public class SecondJWTMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public SecondJWTMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public Task Invoke(HttpContext httpContext)
//        {

//            return _next(httpContext);
//        }
//    }

//    // Extension method used to add the middleware to the HTTP request pipeline.
//    public static class SecondJWTMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseSecondJWTMiddleware(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<SecondJWTMiddleware>();
//        }
//    }
//}
