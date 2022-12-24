using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class Added_Dislike_Field_UserLikesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDislike",
                table: "UserLikes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDislike",
                table: "UserLikes");
        }
    }
}
