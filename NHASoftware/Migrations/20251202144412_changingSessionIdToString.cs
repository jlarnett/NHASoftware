using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHA.Website.Software.Migrations
{
    /// <inheritdoc />
    public partial class changingSessionIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "HiddenPosts",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "HiddenPosts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36);
        }
    }
}
