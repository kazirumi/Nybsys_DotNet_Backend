
using DotNet6Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet6Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDBContext _context;

        public UserController(UserManager<ApplicationUser> userManager,ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        [Authorize(Roles ="Admin")]
        //GET : /api/User
        public async Task<List<ApplicationUser>> GetUserList()
        {
            var UserList = await _userManager.Users.ToListAsync();

            foreach (var user in UserList)
            {
                user.Role = string.Join(",", _userManager.GetRolesAsync(user).Result.ToArray());
            }
            return UserList;
          
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        //GET : /api/User
        public async Task<ActionResult> EditUser(string id,RequestUserModel userModel)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("user not found");
            }
            user.FullName = userModel.FullName;
            user.UserName = userModel.UserName;
            user.Email = userModel.Email;

            await _userManager.UpdateAsync(user);

            return Ok(new { message = "user updated successfully" });

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        //GET : /api/User
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user==null)
            {
                return NotFound("user not found");
            }

            await _userManager.DeleteAsync(user);
            return Ok(new { message="user deleted successfully" });

        }
    }
}
