using System.ComponentModel.DataAnnotations;

namespace Epsic.Info3e.Mays.Models
{
    public class CreditCard
    {
        [Required]
        [RegularExpression("^[0-9]{16}$")]
        public string CardNumber { get; set; }
        [Required]
        [Range(1, 12)]
        public int Mount { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Holder { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3}$")]
        public string SecurityCode { get; set; }
        [Required]
        public bool AutoRenew { get; set; }
    }
}