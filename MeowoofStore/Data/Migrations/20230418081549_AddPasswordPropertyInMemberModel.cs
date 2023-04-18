using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordPropertyInMemberModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "Member",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "Member");
        }
    }
}
