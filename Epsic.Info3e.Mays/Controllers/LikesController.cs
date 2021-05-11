using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Epsic.Info3e.Mays.Services;

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
        public ActionResult<Like> AllLike(string postId)
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
        public IActionResult RemoveLike(string postId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;

            _service.RemoveLike(userId, postId);

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public ActionResult<Like> GetUserLikes(string userId)
        {
            return Ok(_service.GetUserLikes(userId));
        }

        [HttpGet("post/{postId}")]
        public ActionResult<Like> GetPostLikes(string postId)
        {
            return Ok(_service.GetPostLikes(postId));
        }
    }
}
