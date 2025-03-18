using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.DAL
{
    public class AppDbContext: IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<HistoricalStockPrice> HistoricalStockPrices { get; set; } //Add the HistoricalStockPrice here.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important if inheriting from IdentityDbContext

            // Configure relationships (Foreign Keys)
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.User)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Portfolio)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.PortfolioId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany() // Assuming one-to-many relationship with User
                .HasForeignKey(n => n.UserId);
        }
    }
}

