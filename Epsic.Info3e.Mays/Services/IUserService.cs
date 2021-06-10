using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Epsic.Info3e.Mays.Services
{
    public interface IUserService
    {
        public Task<FullUserDto> GetUser(string id);
        public Task<bool> ChangePassword(ChangePassword changePassword, string userId);
        public Task<string> SaveFileAsync(AvatarUpload avatar, string userId);
    }
}
