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
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;

        public PostsController(UserManager<User> userManager,
                               IPostService postService)
        {
            _userManager = userManager;
            _postService = postService;
        }

        // GET: api/Posts
        [HttpGet]
        /// <summary>
        /// Returns a list of posts
        /// </summary>
        /// <returns>The list of posts</returns>
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            return (await _postService.GetPostsAsync())
                .Select(post => _postService.ToPostDto(post, getCurrentUserId()))
                .ToList();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        /// <summary>
        /// Returns a single post
        /// </summary>
        /// <param name="id">Id of the post to get</param>
        /// <returns>The post if it exists, or notfound</returns>
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
        /// <summary>
        /// Updates a post
        /// </summary>
        /// <param name="id">Id of the post to update</param>
        /// <param name="post">New post data</param>
        /// <returns>Nocontent on success, badrequest and status 500 on error, or notfound if the post does not exist</returns>
        public async Task<IActionResult> PutPost(string id, Post post)
        {
            try
            {
                // TODO: use user id
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
        /// <summary>
        /// Creates a post
        /// </summary>
        /// <param name="post">Post to create</param>
        /// <returns>Badrequest if there is no content, createdataction on success, or statuscode on error</returns>
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
        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="id">Id of the post to delete</param>
        /// <returns>Nocontent on deletion, forbid if not allowed, notfound if the post does not exist</returns>
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

        /// <summary>
        /// Returns the id of the current user
        /// </summary>
        /// <returns>The id of the current user if exists, or null</returns>
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
