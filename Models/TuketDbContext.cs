using Microsoft.EntityFrameworkCore;

namespace TuketAppAPI.Models
{
    public class TuketDbContext : DbContext
    {
        public TuketDbContext(DbContextOptions<TuketDbContext> options) : base(options) { }

        // ✅ Kullanıcılar tablosu
        public DbSet<User> Users { get; set; }

        // ✅ İşletmeler tablosu
        public DbSet<Business> Businesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Business>().ToTable("businesses");
        }
    }
}