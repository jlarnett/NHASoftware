using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHA.Website.Software.Migrations
{
    /// <inheritdoc />
    public partial class featuredBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Featured",
                table: "AnimePages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Featured",
                table: "AnimePages");
        }
    }
}
