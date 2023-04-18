using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulateRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[Role] ([id], [roleName]) VALUES (1, N'Administrator')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Role] ([id], [roleName]) VALUES (2, N'Customer')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
