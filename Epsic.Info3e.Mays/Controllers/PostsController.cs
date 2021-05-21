using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Epsic.Info3e.Mays.Services;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPostService _postService;

        public PostsController(UserManager<IdentityUser> userManager,
                               IPostService postService)
        {
            _userManager = userManager;
            _postService = postService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            return (await _postService.GetPostsAsync())
                .Select(post => _postService.ToPostDto(post, getCurrentUserId()))
                .ToList();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(string id)
        {
            var post = await _postService.GetPostAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return _postService.ToPostDto(post, getCurrentUserId());
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(string id, Post post)
        {
            try
            {
                if (await _postService.UpdatePostAsync(id, post))
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "user,premium,admin")]
        public async Task<ActionResult<PostDto>> PostPost(Post post)
        {
            if (post.Content.Length == 0 && post.FileContent.Length == 0)
            {
                return BadRequest();
            }

            post.Author = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);
            post.Date = DateTime.Now;

            if (await _postService.AddPostAsync(post, User))
            {
                var result = _postService.ToPostDto(await _postService.GetPostAsync(post.Id), getCurrentUserId());
                return CreatedAtAction("GetPost", new { id = post.Id }, post);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            try
            {
                if (await _postService.DeletePostAsync(id, User))
                {
                    return NoContent();
                }
                else
                {
                    return Forbid();
                }
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        private string getCurrentUserId()
        {
            if (User.Claims.Any(x => x.Type == "Id"))
            {
                return User.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            }
            return null;
        }
    }
}
