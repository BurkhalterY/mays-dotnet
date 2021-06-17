using System.Threading.Tasks;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity;

namespace Epsic.Info3e.Mays.Seeders
{
    public static class SeedDataApplicationPosts
    {
        private class FakePost
        {
            public string Title { get; set; }
            public string AuthorName { get; set; }
            public string Content { get; set; } = null;
            public string FileName { get; set; } = null;
            public string FileType { get; set; } = null;
            public string IsSpoiler { get; set; }
        }

        public static async Task SeedPostsAsync(UserManager<User> userManager, MaysDbContext context)
        {
            var posts = new FakePost[] {
                new FakePost {
                    Title = "Nouveau site",
                    AuthorName = "admin",
                    Content = "On a un nouveau site",
                },
            };

            foreach (var post in posts)
            {
                var author = await userManager.FindByNameAsync(post.AuthorName);
                if (author == null) continue;

                var realPost = new Post {
                    Title = post.Title,
                    Author = author,
                };
                var valid = false;

                if (post.Content != null)
                {
                    realPost.Content = post.Content;
                    valid = true;
                }
                if (post.FileName != null && post.FileType != null)
                {
                    realPost.FileName = post.FileName;
                    realPost.FileType = post.FileType;
                    valid = true;
                }

                if (!valid) continue;

                context.Posts.Add(realPost);
                context.SaveChanges();
            }
        }
    }
}
