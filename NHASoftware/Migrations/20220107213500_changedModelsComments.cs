using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class changedModelsComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_AspNetUsers_UserId1",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost");

            migrationBuilder.DropIndex(
                name: "IX_ForumComments_UserId1",
                table: "ForumComments");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ForumComments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ForumPost",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ForumComments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComments_UserId",
                table: "ForumComments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_AspNetUsers_UserId",
                table: "ForumComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_AspNetUsers_UserId",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost");

            migrationBuilder.DropIndex(
                name: "IX_ForumComments_UserId",
                table: "ForumComments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ForumPost",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ForumComments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "ForumComments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumComments_UserId1",
                table: "ForumComments",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_AspNetUsers_UserId1",
                table: "ForumComments",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumPost_AspNetUsers_UserId",
                table: "ForumPost",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
