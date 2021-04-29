using System.ComponentModel.DataAnnotations;

namespace Epsic.Info3e.Mays.Models
{
    public class LoginRequest
    {
        [Required]
        public string Input { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
