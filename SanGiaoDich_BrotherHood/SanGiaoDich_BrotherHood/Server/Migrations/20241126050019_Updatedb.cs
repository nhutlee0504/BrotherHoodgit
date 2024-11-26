using Microsoft.EntityFrameworkCore.Migrations;

namespace SanGiaoDich_BrotherHood.Server.Migrations
{
    public partial class Updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_Accounts_AccountUserName",
                table: "PaymentRequests");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequests_AccountUserName",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "AccountUserName",
                table: "PaymentRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "PaymentRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_UserName",
                table: "PaymentRequests",
                column: "UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Accounts_UserName",
                table: "PaymentRequests",
                column: "UserName",
                principalTable: "Accounts",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_Accounts_UserName",
                table: "PaymentRequests");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequests_UserName",
                table: "PaymentRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "PaymentRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountUserName",
                table: "PaymentRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_AccountUserName",
                table: "PaymentRequests",
                column: "AccountUserName");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Accounts_AccountUserName",
                table: "PaymentRequests",
                column: "AccountUserName",
                principalTable: "Accounts",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
