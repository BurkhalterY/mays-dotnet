using Microsoft.AspNetCore.Identity;

namespace Epsic.Info3e.Mays.Models
{
    public class User : IdentityUser
    {
        public string Avatar { get; set; }
    }

    public class UserDto
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }

    public class FullUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}
