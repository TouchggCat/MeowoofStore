using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressInMember_AddOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "roleName",
                table: "Role",
                newName: "RoleName");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Member",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "memberName",
                table: "Member",
                newName: "MemberName");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Member",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Member",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Address",
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
                name: "Address",
                table: "Member");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "Role",
                newName: "roleName");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Member",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "MemberName",
                table: "Member",
                newName: "memberName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Member",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Member",
                newName: "id");
        }
    }
}
