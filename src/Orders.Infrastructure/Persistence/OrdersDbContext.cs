using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).HasColumnName("id");
            entity.Property(o => o.CustomerId).HasColumnName("customer_id").HasMaxLength(100).IsRequired();
            entity.Property(o => o.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2).IsRequired();
            entity.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
        });
    }
}
