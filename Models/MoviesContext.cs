using Microsoft.EntityFrameworkCore;

namespace IdentityFromScratch.Models
{
    public class MoviesContext : DbContext
    {
        public MoviesContext(DbContextOptions<MoviesContext> options)
 : base(options) { }
        public DbSet<Movies> Movies { get; set; }
        public DbSet<Watch> Watch { get; set; }
    }
}
