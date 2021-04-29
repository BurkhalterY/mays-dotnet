using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(MaysDbContext context,
                               IAuthorizationService authorizationService,
                               IWebHostEnvironment environment,
                               ILogger<PostsController> logger,
                               UserManager<IdentityUser> userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _environment = environment;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            return await _context.Posts.Select(post => new PostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Date = post.Date,
                Author = new UserDto() { UserName = post.Author.UserName },
                Content = post.Content,
                FilePath = post.FilePath,
                IsSpoiler = post.IsSpoiler
            }).ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(string id)
        {
            var post = await _context.Posts.Select(post => new PostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Date = post.Date,
                Author = new UserDto() { UserName = post.Author.UserName },
                Content = post.Content,
                FilePath = post.FilePath,
                IsSpoiler = post.IsSpoiler
            }).FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(string id, Post post)
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
        public async Task<ActionResult<PostDto>> PostPost(Post post)
        {
            if (post.FileContent != null)
            {
                var filePath = await SaveFileAsync(post.FileContent, post.FileName);
                if (filePath == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                post.FilePath = filePath;
            }

            post.Author = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);
            post.Date = DateTime.Now;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var authorizationResultAdmin = await _authorizationService.AuthorizeAsync(User, "Admin");
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

        private bool PostExists(string id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        /// <summary>
        /// Saves a file to disc.
        /// </summary>
        /// <param name="file">File to save to the disc</param>
        /// <param name="filename">Name of the file to save</param>
        /// <returns>The name of the file if saved, null otherwise</returns>
        private async Task<string> SaveFileAsync(byte[] fileContent, string fileName)
        {
            try
            {
                if (!fileName.Contains("."))
                {
                    return null;
                }

                var filePath = $"{_environment.WebRootPath}\\Assets\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var extension = fileName.Split('.').Last();
                fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName = Regex.Replace(fileName, "/[^a-zA-Z0-9]+/g", "-");

                if (System.IO.File.Exists($"{filePath}{fileName}.{extension}"))
                {
                    var suffix = 0;
                    while (System.IO.File.Exists($"{filePath}{fileName}-{suffix}.{extension}"))
                    {
                        suffix++;
                    }

                    fileName += $"-{suffix}";
                }

                fileName += $".{extension}";

                using FileStream fs = System.IO.File.Create($"{filePath}{fileName}");
                await fs.WriteAsync(fileContent);
                fs.Flush();

                return fileName;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }
    }
}
