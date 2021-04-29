using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epsic.Info3e.Mays.Models
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public IdentityUser Author { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string FileName { get; set; }
        [NotMapped]
        public byte[] FileContent { get; set; }
        public string FilePath { get; set; }
        public bool IsSpoiler { get; set; }
    }
}
