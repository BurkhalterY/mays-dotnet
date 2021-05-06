using System.Collections.Generic;
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
        /// <param name="postId">Id of the post to update</param>
        /// <param name="post">Post to update</param>
        /// <returns>True if the post is updated, false otherwise</returns>
        public Task<bool> UpdatePostAsync(string postId, Post post);

        /// <summary>
        /// Adds a post
        /// </summary>
        /// <param name="post">Post to add</param>
        /// <returns>True if the post is added, false otherwise</returns>
        public Task<bool> AddPostAsync(Post post, ClaimsPrincipal user);

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="postId">Id of the post to delete</param>
        public Task<bool> DeletePostAsync(string postId, ClaimsPrincipal user);

        public PostDto ToPostDto(Post post)
        {
            return new PostDto {
                Id = post.Id,
                Title = post.Title,
                Date = post.Date,
                Author = new UserDto{ UserName = post.Author.UserName },
                Content = post.Content,
                FilePath = post.FilePath,
                IsSpoiler = post.IsSpoiler,
            };
        }
    }
}
