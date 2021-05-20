using System.ComponentModel.DataAnnotations;

namespace Epsic.Info3e.Mays.Models
{
    public class ChangePassword
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}