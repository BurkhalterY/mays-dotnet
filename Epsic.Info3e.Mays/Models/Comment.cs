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
        [Required]
        public string PostId { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
        [JsonIgnore]
        public User Author { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsSpoiler { get; set; }
    }

    public class CommentUpdate
    {
        [JsonIgnore]
        public string Id { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsSpoiler { get; set; }
    }

    public class CommentDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public PostDto Post { get; set; }
        public UserDto Author { get; set; }
        public string Content { get; set; }
        public bool IsSpoiler { get; set; }
    }
}
