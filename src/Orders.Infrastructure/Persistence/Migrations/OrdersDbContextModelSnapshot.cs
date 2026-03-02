using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Orders.Infrastructure.Persistence.Migrations;

[DbContext(typeof(OrdersDbContext))]
partial class OrdersDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

        modelBuilder.Entity("Orders.Domain.Entities.Order", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("TEXT")
                .HasColumnName("id");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT")
                .HasColumnName("created_at");

            b.Property<string>("CustomerId")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("TEXT")
                .HasColumnName("customer_id");

            b.Property<decimal>("TotalAmount")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT")
                .HasColumnName("total_amount");

            b.HasKey("Id");

            b.ToTable("orders", (string)null);
        });
#pragma warning restore 612, 618
    }
}
