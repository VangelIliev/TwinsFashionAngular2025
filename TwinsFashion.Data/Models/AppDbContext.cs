using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TwinsFashion.Data.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Color> Colors => Set<Color>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<SubCategory> SubCategories => Set<SubCategory>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.SubCategories)
            .WithOne(sc => sc.Category)
            .HasForeignKey(sc => sc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubCategory>()
            .HasMany(sc => sc.Products)
            .WithOne(p => p.SubCategory)
            .HasForeignKey(p => p.SubCategoryId);

        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId });

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Sizes)
            .WithMany(s => s.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductSize",
                j => j
                    .HasOne<Size>()
                    .WithMany()
                    .HasForeignKey("SizeId")
                    .HasConstraintName("FK_ProductSize_Size_SizeId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .HasConstraintName("FK_ProductSize_Product_ProductId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("ProductId", "SizeId");
                    j.ToTable("ProductSize");
                });

        modelBuilder.Entity<Product>()
            .HasOne(p => p.SubCategory)
            .WithMany(sc => sc.Products)
            .HasForeignKey(p => p.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
