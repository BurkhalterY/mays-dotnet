using Microsoft.AspNetCore.Identity;
using System;
using System.Text.Json.Serialization;

namespace Epsic.Info3e.Mays.Models
{
    public class User : IdentityUser
    {
        public string Avatar { get; set; }
        [JsonIgnore]
        public DateTime ExpirationDate { get; set; }
        [JsonIgnore]
        public bool AutoRenew { get; set; }
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
        public bool IsPremium { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
