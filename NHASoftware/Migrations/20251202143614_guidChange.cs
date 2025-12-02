using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHA.Website.Software.Migrations
{
    /// <inheritdoc />
    public partial class guidChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HiddenPosts_AspNetUsers_UserId",
                table: "HiddenPosts");

            migrationBuilder.DropIndex(
                name: "IX_HiddenPosts_UserId",
                table: "HiddenPosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "HiddenPosts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "HiddenPosts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_HiddenPosts_UserId",
                table: "HiddenPosts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HiddenPosts_AspNetUsers_UserId",
                table: "HiddenPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
