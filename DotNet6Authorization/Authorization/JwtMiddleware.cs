using DotNet6Authorization.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DotNet6Authorization.Authorization
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationSettings _appSettings;
        private readonly ApplicationDBContext _dBContext;

        //private readonly UserManager<ApplicationUser> _userManager;

        public JwtMiddleware(RequestDelegate next, IOptions<ApplicationSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
            
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils,UserManager<ApplicationUser> _userManager)
        {

            string path = context.Request.Path.Value;
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                if (IsEnableUnauthorizedRoute(path))
                {
                    await _next(context);
                }
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            else
            {
                var userId = jwtUtils.ValidateJwtToken(token);
                if (userId != null)
                {
                    //attach user to context on successful jwt validation
                    context.Items["User"] = _userManager.FindByIdAsync(userId);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }

            

            await _next(context);
        }

        private bool IsEnableUnauthorizedRoute(string path)
        {
            List<string> UnauthorizedRoutes = new List<string>
            {
                "/api/ApplicationUsers/Login",
                "/api/ApplicationUsers/Register"
            };

            if(UnauthorizedRoutes.Contains(path))
                return true;

            return false;   
        }
    }

    //Extension method used to add the middleware to the HTTP request pipeline.
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
