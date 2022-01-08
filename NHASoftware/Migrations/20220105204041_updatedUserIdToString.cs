using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class updatedUserIdToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId1",
                table: "ForumPost");

            migrationBuilder.DropIndex(
                name: "IX_ForumPost_UserId1",
                table: "ForumPost");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ForumPost");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ForumPost",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPost_UserId",
                table: "ForumPost",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost");

            migrationBuilder.DropIndex(
                name: "IX_ForumPost_UserId",
                table: "ForumPost");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ForumPost",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "ForumPost",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumPost_UserId1",
                table: "ForumPost",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId1",
                table: "ForumPost",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
