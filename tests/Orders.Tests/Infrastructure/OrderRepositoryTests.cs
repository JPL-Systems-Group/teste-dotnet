using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Repositories;

namespace Orders.Tests.Infrastructure;

public sealed class OrderRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistOrder()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new OrdersDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new OrderRepository(context);
        var order = Order.Create("customer-3", 22);

        await repository.AddAsync(order);
        var loaded = await repository.GetByIdAsync(order.Id);

        loaded.Should().NotBeNull();
        loaded!.CustomerId.Should().Be("customer-3");
    }
}
