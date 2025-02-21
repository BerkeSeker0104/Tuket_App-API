using Microsoft.EntityFrameworkCore;

namespace TuketAppAPI.Models
{
    public class TuketDbContext : DbContext
    {
        public TuketDbContext(DbContextOptions<TuketDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<BusinessReview> BusinessReviews { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Business>().ToTable("businesses");
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Order>().ToTable("orders");
            modelBuilder.Entity<OrderItem>().ToTable("order_items");
            modelBuilder.Entity<BusinessReview>().ToTable("business_reviews");
            modelBuilder.Entity<ProductReview>().ToTable("product_reviews");
        }
    }
}