using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoPriceTracker.Infrastructure.Data.Migrations
{
    public partial class IconUrlToCryptoAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "CryptoAssets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "CryptoAssets",
                keyColumn: "Id",
                keyValue: 1,
                column: "IconUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "CryptoAssets",
                keyColumn: "Id",
                keyValue: 2,
                column: "IconUrl",
                value: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "CryptoAssets");
        }
    }
}
