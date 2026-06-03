using Microsoft.EntityFrameworkCore;
using VegetableShopApi.Models;

namespace VegetableShopApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Name).HasColumnName("name");
            entity.Property(c => c.IconUrl).HasColumnName("icon_url");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Name).HasColumnName("name");
            entity.Property(p => p.Price).HasColumnName("price").HasPrecision(10, 2);;
            entity.Property(p => p.ImageUrl).HasColumnName("image_url");
            entity.Property(p => p.StockQuantity).HasColumnName("stock_quantity").HasPrecision(10, 2);;
            entity.Property(p => p.CategoryId).HasColumnName("category_id");
            entity.Property(p => p.Unit).HasColumnName("unit").HasColumnType("unit");
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        });
        
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(10, 2);;
            entity.Property(e => e.Unit).HasColumnName("unit").HasColumnType("unit");
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasPrecision(10, 2);;
            entity.Property(e => e.TotalPrice).HasColumnName("total_price").HasPrecision(10, 2);;
            entity.HasOne(e => e.Order).WithMany(o => o.Items).HasForeignKey(e => e.OrderId);
            entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DeliveryAddress).HasColumnName("delivery_address");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price").HasPrecision(10, 2);;
            entity.Property(e => e.Status).HasColumnName("status").HasColumnType("order_status");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("firstname");
            entity.Property(e => e.LastName).HasColumnName("lastname");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role").HasColumnType("role");
        });
        

    }
}