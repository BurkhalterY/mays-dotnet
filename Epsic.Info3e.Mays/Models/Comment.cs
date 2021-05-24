using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Epsic.Info3e.Mays.Models
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [JsonIgnore]
        public User Author { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsSpoiler { get; set; }
    }

    public class CommentDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public UserDto Author { get; set; }
        public string Content { get; set; }
        public bool IsSpoiler { get; set; }
    }
}
