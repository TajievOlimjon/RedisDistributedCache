using Microsoft.EntityFrameworkCore;

namespace WebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}



