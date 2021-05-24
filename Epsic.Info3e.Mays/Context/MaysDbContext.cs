using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Epsic.Info3e.Mays.DbContext
{
    public class MaysDbContext : IdentityDbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public MaysDbContext(DbContextOptions<MaysDbContext> options) : base(options)
        {

        }

    }
}
