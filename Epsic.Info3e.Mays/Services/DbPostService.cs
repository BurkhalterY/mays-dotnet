using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Authorization;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epsic.Info3e.Mays.Services
{
    class DbPostService : IPostService
    {
        private readonly MaysDbContext _context;
        private readonly ILogger<DbPostService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IWebHostEnvironment _environment;

        public DbPostService(MaysDbContext context,
                             ILogger<DbPostService> logger,
                             IAuthorizationService authorizationService,
                             IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _authorizationService = authorizationService;
            _environment = environment;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _context.Posts.Include(p => p.Author).ToListAsync();
        }

        public async Task<Post> GetPostAsync(string postId)
        {
            if (_context.Posts.Any(p => p.Id == postId))
            {
                return await _context.Posts.Include(p => p.Author).FirstAsync(p => p.Id == postId);
            }

            return null;
        }

        public async Task<bool> UpdatePostAsync(string postId, Post post)
        {
            if (postId != post.Id)
            {
                return false;
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!await PostExistsAsync(postId))
                {
                    throw new NullReferenceException();
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> AddPostAsync(Post post, ClaimsPrincipal user)
        {
            if (post.FileContent != null)
            {
                var filePath = await SaveFileAsync(post.FileContent, post.FileName, user);
                if (filePath == null)
                {
                    return false;
                }

                post.FilePath = filePath;
            }
            else
            {
                // Just to make sure there is no file linked
                post.FilePath = null;
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePostAsync(string postId, ClaimsPrincipal user)
        {
            if (!await PostExistsAsync(postId))
            {
                throw new NullReferenceException();
            }

            var post = await _context.Posts.FindAsync(postId);

            var allowed = (await _authorizationService.AuthorizeAsync(user, "Admin")).Succeeded ||
                (await _authorizationService.AuthorizeAsync(user, "SameUserPost")).Succeeded;

            if (allowed)
            {
                //todo delete likes
                //todo delete file
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            return true;
        }

        private async Task<bool> PostExistsAsync(string postId)
        {
            return await _context.Posts.AnyAsync(p => p.Id == postId);
        }

        private async Task<string> SaveFileAsync(byte[] fileContent, string fileName, ClaimsPrincipal user)
        {
            try
            {
                if (!fileName.Contains('.'))
                {
                    return null;
                }

                var filePath = $"{_environment.WebRootPath}\\Assets\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var extension = fileName.Split('.').Last();
                var allowed = (await _authorizationService.AuthorizeAsync(user, extension, "Extension")).Succeeded;

                if (!allowed)
                {
                    return null;
                }

                fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName = Regex.Replace(fileName, "/[^a-zA-Z0-9]+/g", "-");

                if (File.Exists($"{filePath}{fileName}.{extension}"))
                {
                    var suffix = 0;
                    while (File.Exists($"{filePath}{fileName}-{suffix}.{extension}"))
                    {
                        suffix++;
                    }

                    fileName += $"-{suffix}";
                }

                fileName += $".{extension}";

                using FileStream fs = File.Create($"{filePath}{fileName}");
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
