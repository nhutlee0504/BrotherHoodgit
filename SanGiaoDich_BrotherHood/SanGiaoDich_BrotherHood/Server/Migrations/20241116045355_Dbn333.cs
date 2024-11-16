using Microsoft.EntityFrameworkCore.Migrations;

namespace SanGiaoDich_BrotherHood.Server.Migrations
{
    public partial class Dbn333 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ImageProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ImageProducts");
        }
    }
}
