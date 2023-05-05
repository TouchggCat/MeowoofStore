using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsShoppingToIsShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsShopping",
                table: "Order",
                newName: "IsShipping");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsShipping",
                table: "Order",
                newName: "IsShopping");
        }
    }
}
