using FoodHub.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug).IsUnique();

        modelBuilder.Entity<Restaurant>(e =>
        {
            e.HasIndex(r => r.Slug).IsUnique();               // مفيش مطعمين بنفس الـ URL
            e.Property(r => r.DeliveryFee).HasPrecision(10, 2);
            e.Property(r => r.MinOrder).HasPrecision(10, 2);

            e.HasOne(r => r.Category)
             .WithMany(c => c.Restaurants)
             .HasForeignKey(r => r.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);               // ممنوع تمسح Category فيها مطاعم
        });

        modelBuilder.Entity<MenuItem>(e =>
        {
            e.Property(m => m.Price).HasPrecision(10, 2);
            e.HasOne(m => m.Restaurant)
             .WithMany(r => r.MenuItems)
             .HasForeignKey(m => m.RestaurantId)
             .OnDelete(DeleteBehavior.Cascade);                // المطعم يتمسح → المنيو تتمسح
        });

        modelBuilder.Entity<Review>()
            .HasOne(rv => rv.Restaurant)
            .WithMany(r => r.Reviews)
            .HasForeignKey(rv => rv.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData.Seed(modelBuilder);
    }
}
