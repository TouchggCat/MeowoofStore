using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulateDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[Member] ([Id], [Email], [MemberName], [RoleId], [Password], [Address], [Salt], [IsEnabled]) VALUES (1, N'admin@admin.com', N'MeowoofStoreAdmin', 1, N'oLbL+sWSIeBKGuw5vzR3e0t/0nP090MkMygXAneBU8c=', N'106台北市大安區臥龍街100號', 0xBE193FC87EDD091A6454153E6C023041, 1)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Member] ([Id], [Email], [MemberName], [RoleId], [Password], [Address], [Salt], [IsEnabled]) VALUES (2, N'customer@customer.com', N'Customer_No1', 2, N'UK6HOJ3feWk3LnUY2fJzMRIPzdiMbq8+BUFEbLnQa3M=', N'110台北市信義區松仁路1號', 0x61132F536987D30F1834F062A7E53E19, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
