using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Epsic.Info3e.Mays.Models
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [JsonIgnore]
        public IdentityUser Author { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string FileName { get; set; }
        [NotMapped]
        public byte[] FileContent { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public bool IsSpoiler { get; set; }
    }

    public class PostDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public UserDto Author { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public bool IsSpoiler { get; set; }
    }
}
