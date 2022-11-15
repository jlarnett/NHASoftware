using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class NewAnimeModelKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnimePageId",
                table: "AnimeEpisodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AnimeEpisodes_AnimePageId",
                table: "AnimeEpisodes",
                column: "AnimePageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimeEpisodes_AnimePages_AnimePageId",
                table: "AnimeEpisodes",
                column: "AnimePageId",
                principalTable: "AnimePages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimeEpisodes_AnimePages_AnimePageId",
                table: "AnimeEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_AnimeEpisodes_AnimePageId",
                table: "AnimeEpisodes");

            migrationBuilder.DropColumn(
                name: "AnimePageId",
                table: "AnimeEpisodes");
        }
    }
}
