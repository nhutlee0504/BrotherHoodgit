using Microsoft.EntityFrameworkCore.Migrations;

namespace SanGiaoDich_BrotherHood.Server.Migrations
{
    public partial class dbn001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageID",
                table: "ConversationParticipants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipants",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_MessageID",
                table: "ConversationParticipants",
                column: "MessageID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Conversations_ConversationId",
                table: "ConversationParticipants",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Messages_MessageID",
                table: "ConversationParticipants",
                column: "MessageID",
                principalTable: "Messages",
                principalColumn: "MessageID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Conversations_ConversationId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Messages_MessageID",
                table: "ConversationParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_MessageID",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "MessageID",
                table: "ConversationParticipants");
        }
    }
}
