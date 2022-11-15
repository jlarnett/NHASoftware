using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class NewAnimeModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimeEpisodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpisodeNumber = table.Column<int>(type: "int", nullable: false),
                    EpisodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EpisodeSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EpisodeContainsFiller = table.Column<bool>(type: "bit", nullable: false),
                    UpVotes = table.Column<int>(type: "int", nullable: false),
                    DownVotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeEpisodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimePages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnimeSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpVotes = table.Column<int>(type: "int", nullable: false),
                    DownVotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimePages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimeEpisodes");

            migrationBuilder.DropTable(
                name: "AnimePages");
        }
    }
}
