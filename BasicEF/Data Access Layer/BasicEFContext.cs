using BasicEF.Entites;
using Microsoft.EntityFrameworkCore;

namespace BasicEF.Data_Access_Layer
{
    public class BasicEFContext : DbContext
    {
        public BasicEFContext(DbContextOptions<BasicEFContext> options) : base(options)
        {
        }

        public DbSet<student> students { get; set; } = default!;
    }
}
