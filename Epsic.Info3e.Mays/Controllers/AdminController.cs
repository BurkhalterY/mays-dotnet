using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MaysDbContext _context;

        public AdminController(UserManager<User> userManager, MaysDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("Enable/{userId}")]
        /// <summary>
        /// Makes an user premium
        /// </summary>
        /// <param name="userId">Id of the user to premium, defaults to the current user if none is selected, or the user is neither premium nor admin</param>
        /// <returns>Badrequest if the target user is already premium, or ok</returns>
        public async Task<IActionResult> Enable(string userId = null)
        {
            if (!User.IsInRole("admin"))
            {
                if (!User.IsInRole("premium"))
                {
                    // Free users can only premium themselves
                    userId ??= User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                }

                //TODO Check for credit card
            }

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any(r => string.Equals(r, "premium", System.StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest();
            }

            await _userManager.AddToRoleAsync(user, "premium");

            user.ExpirationDate = DateTime.Now.AddMonths(1);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Disable/{userId}")]
        /// <summary>
        /// Removes an user from premium
        /// </summary>
        /// <param name="userId">Id of the user to premium, defaults to the current user if they are not admin</param>
        /// <returns>Forbid if the current user is neither admin nor premium, badrequest if the target user is not premium, ok otherwise</returns>
        public async Task<IActionResult> Disable(string userId = null)
        {
            if (!User.IsInRole("admin"))
            {
                if (User.IsInRole("premium"))
                {
                    userId = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                }
                else
                {
                    return Forbid();
                }
            }

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any(r => string.Equals(r, "premium", System.StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest();
            }

            await _userManager.RemoveFromRoleAsync(user, "premium");

            user.ExpirationDate = DateTime.MinValue;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("Users")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> Users()
        {
            return Ok(_userManager.Users.Select(user => ToUserDto(user)).ToList());
        }

        private static AdminUserDto ToUserDto(User user)
        {
            return new AdminUserDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Avatar = user.Avatar,
                IsPremium = user.ExpirationDate >= DateTime.Now,
                ExpirationDate = user.ExpirationDate
            };
        }
    }
}
