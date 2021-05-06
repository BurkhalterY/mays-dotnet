using System.Collections.Generic;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Services
{
    interface IPostService
    {
        /// <summary>
        /// Returns a list of posts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Post> GetPosts();

        /// <summary>
        /// Gets a post by its id
        /// </summary>
        /// <param name="postId">Id of the post to get</param>
        /// <returns></returns>
        public Post GetPost(string postId);

        /// <summary>
        /// Updates a post's data
        /// </summary>
        /// <param name="postId">Id of the post to update</param>
        /// <param name="post">Post to update</param>
        /// <returns>True if the post is updated, false otherwise</returns>
        public bool UpdatePost(string postId, Post post);

        /// <summary>
        /// Adds a post
        /// </summary>
        /// <param name="post">Post to add</param>
        /// <returns>True if the post is added, false otherwise</returns>
        public bool AddPost(Post post);

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="postId">Id of the post to delete</param>
        public void DeletePost(string postId);

        /// <summary>
        /// Checks if a post exists
        /// </summary>
        /// <param name="postId">Id of the post to get</param>
        /// <returns>True if the post exists, false otherwise</returns>
        public bool PostExists(string postId);
    }
}
