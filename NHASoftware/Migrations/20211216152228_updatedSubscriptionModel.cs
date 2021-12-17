using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class updatedSubscriptionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionDay",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionDay",
                table: "Subscriptions");
        }
    }
}
