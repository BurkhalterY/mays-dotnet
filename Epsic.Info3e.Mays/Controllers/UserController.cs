using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;
using Microsoft.AspNetCore.Hosting;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Services;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user,premium,admin")]
    public class UserController : ControllerBase
    {
        private readonly MaysDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserService _userService;

        public UserController(MaysDbContext context, UserManager<User> userManager, IWebHostEnvironment environment, IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
            _userService = userService;
        }

        [HttpGet]
        /// <summary>
        /// Returns the current user
        /// </summary>
        /// <returns>The current user</returns>
        public async Task<ActionResult<FullUserDto>> GetUser()
        {
            var user = await _userService.GetUser(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);

            return Ok(user);
        }

        [HttpPut]
        /// <summary>
        /// Updates an user's password
        /// </summary>
        /// <param name="changePassword">A request with the old password and the new password</param>
        /// <returns>Nocontent if both passwords are valid, badrequest if the new password is invalid, unauthorized if the old password is wrong</returns>
        public async Task<ActionResult> ChangePassword(ChangePassword changePassword)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (await _userManager.CheckPasswordAsync(user, changePassword.OldPassword))
            {
                var success = await _userService.ChangePassword(changePassword, userId);
                if (success)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut]
        [Route("avatar")]
        /// <summary>
        /// Updates an avatar for the current user
        /// </summary>
        /// <param name="avatar">Request with the avatar of the user</param>
        /// <returns>Badrequest if the avatar's filename is bad, status 500 on error, nocontent on success</returns>
        public async Task<ActionResult> Avatar(AvatarUpload avatar)
        {
            var id = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            var success = await _userService.SaveFileAsync(avatar, id);
            if (success != null)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
