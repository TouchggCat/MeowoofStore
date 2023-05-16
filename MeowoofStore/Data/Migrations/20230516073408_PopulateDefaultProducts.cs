using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowoofStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulateDefaultProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'莫比Mobby 貓 鹿肉＆鮭魚 3公斤', N'創新成分適合各年齡層貓咪使用。', 900, 108, N'33599ada-4d2d-42a8-a9d9-9d367593c6fb.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'黑貓侍1.5KG 天然無穀貓糧-雞肉+羊肉+鱉蛋粉', N'讓我們的主子吃健康，毛髮柔順，抵抗力增強', 699, 75, N'34c6d585-03bd-4ab5-922d-d6b4ff5b7c7a.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'怪獸部落 LITOMON 貓用 98%鮮肉主食糧 800g', N'98%肉含量+2%植物與營養品+0%穀類、豆類', 980, 40, N'74b17b23-b404-4683-be25-7fc542dae6ce.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'go! 雞肉鮭魚 成犬 高肉量無穀狗糧 12磅', N'85%肉類蛋白，無穀高含肉量，絕佳適口性！ ', 2550, 15, N'7bbbb5a1-76d3-4716-ab5e-9859a4028566.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'NOW 無穀天然糧-鮮肉成貓(火雞+鴨肉+鮭魚) 16LB', N'零人工添加物零穀物 全然鮮肉製成', 3080, 20, N'f8523635-9125-46ac-9f09-32bc301c5c71.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'PetKind 野胃 天然鮮草肚狗糧 放牧鹿肉-6磅', N'富含7種胺基酸、天然消化酵素的鮮草肚！', 1400, 490, N'aa78e45b-6c4e-41a2-8d66-adee812d7b20.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'BLUE BAY倍力 Animate無穀貓飼料(黃) 1.5kg', N'在地生產無穀高蛋白貓飼料，選擇干貝、鮪魚等鮮味打造', 680, 25, N'938fd981-1ab8-4159-98f0-bdcfc8027917.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'Blue Bay 倍力 Animate無穀貓飼料-原野雞鴨 1.5kg', N'腸胃舒敏：幫助消化吸收，奠定成長發育基礎', 719, 40, N'7f36cfb7-d145-4749-99ca-d09265fd866d.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'Orijen 極緻無穀成幼貓 野牧鮮雞 1.8kg', N'內含2/3生鮮肉,1/3低溫脫水肉，豐富蛋白質照顧貓的健康', 1288, 128, N'3a2e1e15-9457-4649-a285-cebdd43c89f0.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'NUTRAM 紐頓 T24 無穀貓-鮭魚+鱒魚 2kg', N'不含穀類與兼具低升醣平衡的天然食材調配', 990, 1000, N'304cbfd7-8366-4926-a0d2-decf8943bcca.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'第一饗宴 無穀低敏 澳洲羊肉全犬配方(小顆粒)2.3kg', N'精選健康放牧羊 藍莓、覆盆子、蔓越莓添加', 699, 50, N'4bc2eb5f-012e-4d4f-84a7-8033bcd11978.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'紐崔斯 頂級無穀犬+凍乾 火雞肉+雞肉+鮭魚 5Kg', N'加拿大製造－以小批量製造生產方式、來實現卓越的品質控管。', 2525, 25, N'c751dfc3-57da-41d5-bd0f-ff350cd61f96.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'柏萊富 特調全齡犬配方(羊肉+糙米+雞肉) 5磅', N'天然優質羊肉蛋白質，幫助呵護敏感皮毛   不含玉米、小麥、黃豆', 770, 70, N'07e9d685-6eb7-4a14-b300-3bfd92f1160c.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'Annamaet 安娜美廚 狗 無穀冰泉 鮭魚鯡魚12磅', N'皮毛保健、幫助消化 天然無穀', 2440, 45, N'5aa0d83c-9212-4026-baec-55d4d48181a0.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'耐吉斯 Solution 無穀成犬-美國放養火雞肉配方 1.5KG', N'無麩質，不含大豆、玉米與小麥的無穀配方', 415, 35, N'bc1e52a4-411f-4865-91dd-f60f2addff49.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'1+1滴雞精貓肉泥主食罐 6件組', N'使用珍貴滴雞精 無製敏添加物', 324, 120, N'aa6b0511-ec14-4245-a53b-3ffed348c15e.jpg')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Product] ([Name], [Description], [Price], [Stock], [ImageString]) VALUES (N'自然主義 成貓 草本貓糧 2KG', N'無麩質低敏食譜  荷蘭獸醫調配 毛孩均衡食補', 882, 20, N'c84e10b9-df58-453c-a339-aa014904c591.jpg')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
