using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Epsic.Info3e.Mays.DbContext
{
    public class MaysDbContext : IdentityDbContext
    {
        public DbSet<Post> Posts { get; set; }

        public MaysDbContext(DbContextOptions<MaysDbContext> options) : base(options)
        {

        }

        public DbSet<Epsic.Info3e.Mays.Models.Like> Like { get; set; }

        public DbSet<Epsic.Info3e.Mays.Models.Comment> Comment { get; set; }
    }
}
