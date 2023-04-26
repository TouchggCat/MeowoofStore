﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulateDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[Member] ([Email], [MemberName], [RoleId], [Password], [Address], [Salt]) VALUES (N'admin@admin.com', N'MeowoofStoreAdmin', 1, N'oLbL+sWSIeBKGuw5vzR3e0t/0nP090MkMygXAneBU8c=', N'106台北市大安區臥龍街100號', 0xBE193FC87EDD091A6454153E6C023041)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Member] ([Email], [MemberName], [RoleId], [Password], [Address], [Salt]) VALUES ( N'customer@customer.com', N'Customer_No1', 2, N'UK6HOJ3feWk3LnUY2fJzMRIPzdiMbq8+BUFEbLnQa3M=', N'100台北市中正區重慶南路一段122號', 0x61132F536987D30F1834F062A7E53E19)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}