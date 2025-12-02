using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHA.Website.Software.Migrations
{
    /// <inheritdoc />
    public partial class hiddenPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HiddenPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiddenPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiddenPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_HiddenPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HiddenPosts_PostId",
                table: "HiddenPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_HiddenPosts_UserId",
                table: "HiddenPosts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HiddenPosts");
        }
    }
}
