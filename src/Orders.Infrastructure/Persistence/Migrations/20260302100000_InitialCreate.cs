using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "orders",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                customer_id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                total_amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_orders", x => x.id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "orders");
    }
}
