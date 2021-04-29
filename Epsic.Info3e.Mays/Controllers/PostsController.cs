using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.RegularExpressions;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly MaysDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PostsController> _logger;

        public PostsController(MaysDbContext context,
                               IAuthorizationService authorizationService,
                               IWebHostEnvironment environment,
                               ILogger<PostsController> logger)
        {
            _context = context;
            _authorizationService = authorizationService;
            _environment = environment;
            _logger = logger;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "user,premium,admin")]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            post.Date = DateTime.Now;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var authorizationResultAdmin = await _authorizationService.AuthorizeAsync(User, post, "Admin");
            var authorizationResultSameUser = await _authorizationService.AuthorizeAsync(User, post, "SameUser");

            if (authorizationResultAdmin.Succeeded || authorizationResultSameUser.Succeeded)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            else
            {
                return Forbid();
            }

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        /// <summary>
        /// Saves a file to disc.
        /// </summary>
        /// <param name="file">File to save to the disc</param>
        /// <param name="filename">Name of the file to save</param>
        /// <returns>True if the file was saved, false otherwise</returns>
        private async Task<bool> SaveFileAsync(IFormFile file, string filename = null)
        {
            try
            {
                if (filename != null && filename == Path.GetFileNameWithoutExtension(filename))
                {
                    filename += $"{file.FileName.Split('.').Last()}";
                }

                filename ??= file.FileName;

                var filepath = $"{_environment.WebRootPath}\\Assets\\";
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var extension = file.FileName.Split('.').Last();
                filename = Path.GetFileNameWithoutExtension(filename);
                filename = Regex.Replace(filename, "/[^a-zA-Z0-9]+/g", "-");

                if (System.IO.File.Exists($"{filepath}{filename}.{extension}"))
                {
                    var suffix = 0;
                    while (System.IO.File.Exists($"{filepath}{filename}-{suffix}.{extension}"))
                    {
                        suffix++;
                    }

                    filename += $"-{suffix}";
                }

                using FileStream fs = System.IO.File.Create($"{filepath}{filename}.{extension}");
                await file.CopyToAsync(fs);
                fs.Flush();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
    }
}
