using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;
using Microsoft.AspNetCore.Hosting;
using Epsic.Info3e.Mays.DbContext;
using System.IO;

namespace Epsic.Info3e.Mays.Services
{
    public class DbUserService : IUserService
    {
        private readonly MaysDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public DbUserService(MaysDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<FullUserDto> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return new FullUserDto() { UserName = user.UserName, Email = user.Email, Avatar = user.Avatar, IsPremium = user.ExpirationDate >= DateTime.Now, ExpirationDate = user.ExpirationDate };
        }

        public async Task<bool> ChangePassword(ChangePassword changePassword, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, changePassword.NewPassword);

            if (result.Succeeded) // If new password is valid
            {
                return true;
            }
            return false;
        }

        public async Task<string> SaveFileAsync(AvatarUpload avatar, string userId)
        {
            try
            {
                if (!avatar.FileName.Contains('.'))
                {
                    return null;
                }

                var filePath = $"{_environment.WebRootPath}\\Avatars\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var extension = avatar.FileName.Split('.').Last().ToLower();

                if (!new string[] { "png", "jpg", "jpeg", "gif", "bmp", "webp" }.ToList().Contains(extension))
                {
                    return null;
                }

                var user = await _userManager.FindByIdAsync(userId);

                // Replace image even if it's the same name because the username is unique

                var fileName = user.UserName + "." + extension;

                using FileStream fs = System.IO.File.Create($"{filePath}{fileName}");
                await fs.WriteAsync(Convert.FromBase64String(avatar.FileContent));
                fs.Flush();

                user.Avatar = fileName;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return fileName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
