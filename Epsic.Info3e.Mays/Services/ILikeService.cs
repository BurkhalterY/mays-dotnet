using System.Collections.Generic;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Services
{
    interface ILikeService
    {
        /// <summary>
        /// Adds a like by an user to a post
        /// </summary>
        /// <param name="userId">Id of the user liking the post</param>
        /// <param name="postId">Id of the post to like</param>
        /// <returns>True if the like has been added, false otherwise</returns>
        public bool AddLike(string userId, string postId);

        /// <summary>
        /// Removes a like by an user to a post
        /// </summary>
        /// <param name="userId">Id of the user unliking the post</param>
        /// <param name="postId">Id of the post to unlike</param>
        /// <returns>True if the like has been removed, false otherwise</returns>
        public bool RemoveLike(string userId, string postId);

        /// <summary>
        /// Gets a list of likes by an user
        /// </summary>
        /// <param name="userId">Id of the user who liked</param>
        /// <returns></returns>
        public IEnumerable<Like> GetUserLikes(string userId);

        /// <summary>
        /// Gets a list of likes on a post
        /// </summary>
        /// <param name="postId">Id of the post liked</param>
        /// <returns></returns>
        public IEnumerable<Like> GetPostLikes(string postId);

        /// <summary>
        /// Checks whether a post has been liked by an user
        /// </summary>
        /// <param name="userId">Id of the user to check for</param>
        /// <param name="postId">Id of the post to check for</param>
        /// <returns>True if the post was liked by the user, false otherwise</returns>
        public bool HasLiked(string userId, string postId);
    }
}
