using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Epsic.Info3e.Mays.Services;
using Microsoft.AspNetCore.Authorization;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly MaysDbContext _context;
        private readonly ILikeService _service;

        public LikesController(MaysDbContext context, ILikeService service)
        {
            _context = context;
            _service = service;
        }

        // POST: api/Likes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "user,premium,admin")]
        /// <summary>
        /// Adds a like to a post from the user
        /// </summary>
        /// <param name="postId">Id of the post liked</param>
        /// <returns>Status 201 on success, or conflict on failure</returns>
        public ActionResult<Like> AddLike(string postId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;

            if (_service.AddLike(userId, postId))
            {
                return StatusCode(201);
            }
            else
            {
                return Conflict();
            }
        }

        // DELETE: api/Likes/5
        [HttpDelete("{postId}")]
        [Authorize(Roles = "user,premium,admin")]
        /// <summary>
        /// Removes a like for a post
        /// </summary>
        /// <param name="postId">Id of the post to remove the like from</param>
        /// <returns>Nocontent, no matter what</returns>
        public IActionResult RemoveLike(string postId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;

            _service.RemoveLike(userId, postId);

            return NoContent();
        }

        // TODO : delete
        [HttpGet("user/{userId}")]
        /// <summary>
        /// Returns a list of likes by the user
        /// </summary>
        /// <param name="userId">Id of the user to get the likes from</param>
        /// <returns>List of likes made by the user</returns>
        public ActionResult<Like> GetUserLikes(string userId)
        {
            return Ok(_service.GetUserLikes(userId));
        }

        // TODO : delete
        [HttpGet("post/{postId}")]
        /// <summary>
        /// Returns a list of likes on post
        /// </summary>
        /// <param name="postId">Id of the post to get the likes from</param>
        /// <returns>The likes put on the posts</returns>
        public ActionResult<Like> GetPostLikes(string postId)
        {
            return Ok(_service.GetPostLikes(postId));
        }
    }
}
