using DotNet6Authorization.Entity;
using DotNet6Authorization.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DotNet6Authorization.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Role> _roles;
      
        public AuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? new Role[] { };
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _userManager = context.HttpContext
                                    .RequestServices
                                    .GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (ApplicationUser)context.HttpContext.Items["User"];

            var user_roles=_userManager.GetRolesAsync(user).Result.ToArray();

            //Comparing string array and enum by convert enum to string
            //var flag = _roles.Any(x => user_roles.Contains(x.ToString()));

            //Comparing string array and enum by convert enum in string
            //var flag = user_roles.Any(x => _roles.Contains((Role)Enum.Parse(typeof(Role), x)));

            if (user == null || _roles.Any() && !_roles.Any(x=> user_roles.Contains(x.ToString())))
            {
                context.Result = new JsonResult(new { message = "unauthorized" }) { StatusCode=StatusCodes.Status401Unauthorized};
            }
        }



        
    }
}
