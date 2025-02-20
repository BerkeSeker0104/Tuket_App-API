using Microsoft.EntityFrameworkCore;

namespace TuketAppAPI.Models
{
    public class TuketDbContext : DbContext
    {
        public TuketDbContext(DbContextOptions<TuketDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Product> Products { get; set; } //  Ürünler eklendi

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Business>().ToTable("businesses");
            modelBuilder.Entity<Product>().ToTable("products"); //  Yeni ürün tablosu
        }
    }
}