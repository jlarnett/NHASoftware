using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class UpdatedForumPostNameToMatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_ForumPost_ForumPostId",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumPost_ForumTopics_ForumTopicId",
                table: "ForumPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ForumPost",
                table: "ForumPost");

            migrationBuilder.RenameTable(
                name: "ForumPost",
                newName: "ForumPosts");

            migrationBuilder.RenameIndex(
                name: "IX_ForumPost_UserId",
                table: "ForumPosts",
                newName: "IX_ForumPosts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ForumPost_ForumTopicId",
                table: "ForumPosts",
                newName: "IX_ForumPosts_ForumTopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForumPosts",
                table: "ForumPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_ForumPosts_ForumPostId",
                table: "ForumComments",
                column: "ForumPostId",
                principalTable: "ForumPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPosts_AspNetUsers_UserId",
                table: "ForumPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPosts_ForumTopics_ForumTopicId",
                table: "ForumPosts",
                column: "ForumTopicId",
                principalTable: "ForumTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_ForumPosts_ForumPostId",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumPosts_AspNetUsers_UserId",
                table: "ForumPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumPosts_ForumTopics_ForumTopicId",
                table: "ForumPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ForumPosts",
                table: "ForumPosts");

            migrationBuilder.RenameTable(
                name: "ForumPosts",
                newName: "ForumPost");

            migrationBuilder.RenameIndex(
                name: "IX_ForumPosts_UserId",
                table: "ForumPost",
                newName: "IX_ForumPost_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ForumPosts_ForumTopicId",
                table: "ForumPost",
                newName: "IX_ForumPost_ForumTopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForumPost",
                table: "ForumPost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_ForumPost_ForumPostId",
                table: "ForumComments",
                column: "ForumPostId",
                principalTable: "ForumPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPost_ForumTopics_ForumTopicId",
                table: "ForumPost",
                column: "ForumTopicId",
                principalTable: "ForumTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
