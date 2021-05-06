using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using System.Collections.Generic;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly MaysDbContext _context;

        public LikesController(MaysDbContext context)
        {
            _context = context;
        }

        // POST: api/Likes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Like>> PostLike(string postId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            var like = new Like() { PostId = postId, UserId = userId };

            if (!LikeExists(like))
            {
                _context.Like.Add(like);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetLike", new { id = like.Id }, like); // TODO
            }
            else
            {
                return Conflict();
            }
        }

        // DELETE: api/Likes/5
        [HttpDelete("{idPost}")]
        public async Task<IActionResult> DeleteLike(string postId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "Id").Value;

            var like = await _context.Like.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
            if (like == null)
            {
                return NotFound();
            }

            _context.Like.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Like>> GetLikes(string userId)
        {
            return _context.Like.Where(l => l.UserId == userId).ToList();
        }

        private bool LikeExists(Like like)
        {
            return _context.Like.Any(e => e.PostId == like.PostId && e.UserId == like.UserId);
        }
    }
}
