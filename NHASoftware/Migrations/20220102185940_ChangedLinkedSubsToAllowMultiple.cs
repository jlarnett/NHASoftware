using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class ChangedLinkedSubsToAllowMultiple : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Subscriptions_SubscriptionId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SubscriptionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "TaskItemId",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_TaskItemId",
                table: "Subscriptions",
                column: "TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Tasks_TaskItemId",
                table: "Subscriptions",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Tasks_TaskItemId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_TaskItemId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TaskItemId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SubscriptionId",
                table: "Tasks",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Subscriptions_SubscriptionId",
                table: "Tasks",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId");
        }
    }
}
