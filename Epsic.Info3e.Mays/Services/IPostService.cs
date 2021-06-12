using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Services
{
    public interface IPostService
    {
        /// <summary>
        /// Returns a list of posts
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Post>> GetPostsAsync();

        /// <summary>
        /// Gets a post by its id
        /// </summary>
        /// <param name="postId">Id of the post to get</param>
        /// <returns></returns>
        public Task<Post> GetPostAsync(string postId);

        /// <summary>
        /// Updates a post's data
        /// </summary>
        /// <param name="userId">Id of the user trying to update</param>
        /// <param name="post">Post to update</param>
        /// <returns>True if the post is updated, false otherwise</returns>
        public Task<bool> UpdatePostAsync(string userId, Post post);

        /// <summary>
        /// Adds a post
        /// </summary>
        /// <param name="post">Post to add</param>
        /// <param name="user">User trying to add</param>
        /// <returns>True if the post is added, false otherwise</returns>
        public Task<bool> AddPostAsync(Post post, ClaimsPrincipal user);

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="postId">Id of the post to delete</param>
        /// <param name="user">User trying to delete</param>
        /// <returns>True on successful deletion, false otherwise</returns>
        public Task<bool> DeletePostAsync(string postId, ClaimsPrincipal user);

        /// <summary>
        /// Gets the comments of a post
        /// </summary>
        /// <param name="postId">Id of the post to get the comments of</param>
        /// <returns>The list of comments in the post</returns>
        public Task<IEnumerable<Comment>> GetPostCommentsAsync(string postId);

        public PostDto ToPostDto(Post post, string userId)
        {
            return new PostDto {
                Id = post.Id,
                Title = post.Title,
                Date = post.Date,
                Author = new UserDto { UserName = post?.Author?.UserName, Avatar = post?.Author?.Avatar },
                Content = post.Content,
                FilePath = post.FilePath,
                FileType = post.FileType,
                IsSpoiler = post.IsSpoiler,
                CountLikes = post.Likes.Count,
                CountComment = post.Comments.Count,
                IsLiked = post.Likes.Any(l => l.UserId == userId),
            };
        }
    }
}
