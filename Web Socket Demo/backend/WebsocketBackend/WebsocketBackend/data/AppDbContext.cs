using Microsoft.EntityFrameworkCore;

namespace WebsocketBackend.data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().HasData(
                new Member { Id = 1, Name = "John Doe" },
                new Member { Id = 2, Name = "Jane Smith" }
            );
        }
    }

    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
