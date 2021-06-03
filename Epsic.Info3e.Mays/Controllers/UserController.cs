using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Epsic.Info3e.Mays.DbContext;

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

        public UserController(MaysDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet]
        /// <summary>
        /// Returns the current user
        /// </summary>
        /// <returns>The current user</returns>
        public async Task<ActionResult<FullUserDto>> GetUser()
        {
            var user = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);

            return new FullUserDto() { UserName = user.UserName, Email = user.Email, Avatar = user.Avatar };
        }

        [HttpPut]
        /// <summary>
        /// Updates an user's password
        /// </summary>
        /// <param name="changePassword">A request with the old password and the new password</param>
        /// <returns>Nocontent if both passwords are valid, badrequest if the new password is invalid, unauthorized if the old password is wrong</returns>
        public async Task<ActionResult> ChangePassword(ChangePassword changePassword)
        {
            var user = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);

            if (await _userManager.CheckPasswordAsync(user, changePassword.OldPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, changePassword.NewPassword);

                if (result.Succeeded) // If new password is valid
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
            try
            {
                if (!avatar.FileName.Contains('.'))
                {
                    return BadRequest();
                }

                var filePath = $"{_environment.WebRootPath}\\Avatars\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var extension = avatar.FileName.Split('.').Last();

                if (!new string[] { "png", "jpg", "jpeg", "gif", "bmp", "webp" }.ToList().Contains(extension))
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);

                // Replace image even if it's the same name because the username is unique

                var fileName = user.UserName + "." + extension;

                using FileStream fs = System.IO.File.Create($"{filePath}{fileName}");
                await fs.WriteAsync(Convert.FromBase64String(avatar.FileContent));
                fs.Flush();

                user.Avatar = fileName;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
