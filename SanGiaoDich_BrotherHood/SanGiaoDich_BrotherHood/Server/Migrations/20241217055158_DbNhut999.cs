using Microsoft.EntityFrameworkCore.Migrations;

namespace SanGiaoDich_BrotherHood.Server.Migrations
{
    public partial class DbNhut999 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "withdrawal_Information",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "withdrawal_Information");
        }
    }
}
