using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHA.Website.Software.Migrations
{
    /// <inheritdoc />
    public partial class updateanime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AnimeName",
                table: "AnimePages",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AnimeGenres",
                table: "AnimePages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AnimeJikanScore",
                table: "AnimePages",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnimeStatus",
                table: "AnimePages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimeGenres",
                table: "AnimePages");

            migrationBuilder.DropColumn(
                name: "AnimeJikanScore",
                table: "AnimePages");

            migrationBuilder.DropColumn(
                name: "AnimeStatus",
                table: "AnimePages");

            migrationBuilder.AlterColumn<string>(
                name: "AnimeName",
                table: "AnimePages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2500)",
                oldMaxLength: 2500);
        }
    }
}
