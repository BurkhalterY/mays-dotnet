using System.ComponentModel.DataAnnotations;

namespace Epsic.Info3e.Mays.Models
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}