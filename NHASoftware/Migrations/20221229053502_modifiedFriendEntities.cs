using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class modifiedFriendEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_AspNetUsers_FriendOneId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_AspNetUsers_FriendTwoId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_FriendOneId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_FriendTwoId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "FriendOneId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "FriendTwoId",
                table: "FriendRequests");

            migrationBuilder.AddColumn<string>(
                name: "RecipientId",
                table: "FriendRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientUserId",
                table: "FriendRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "FriendRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderUserId",
                table: "FriendRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_RecipientId",
                table: "FriendRequests",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_SenderId",
                table: "FriendRequests",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_AspNetUsers_RecipientId",
                table: "FriendRequests",
                column: "RecipientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_AspNetUsers_SenderId",
                table: "FriendRequests",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_AspNetUsers_RecipientId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_AspNetUsers_SenderId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_RecipientId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_SenderId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "RecipientUserId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "SenderUserId",
                table: "FriendRequests");

            migrationBuilder.AddColumn<string>(
                name: "FriendOneId",
                table: "FriendRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FriendTwoId",
                table: "FriendRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_FriendOneId",
                table: "FriendRequests",
                column: "FriendOneId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_FriendTwoId",
                table: "FriendRequests",
                column: "FriendTwoId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_AspNetUsers_FriendOneId",
                table: "FriendRequests",
                column: "FriendOneId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_AspNetUsers_FriendTwoId",
                table: "FriendRequests",
                column: "FriendTwoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
