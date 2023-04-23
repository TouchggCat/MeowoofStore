using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveIsShoppingFromOrderListToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShopping",
                table: "OrderDetail");

            migrationBuilder.AddColumn<bool>(
                name: "IsShopping",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShopping",
                table: "Order");

            migrationBuilder.AddColumn<bool>(
                name: "IsShopping",
                table: "OrderDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
