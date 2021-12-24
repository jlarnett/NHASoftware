using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHASoftware.Migrations
{
    public partial class fixedprimarykeytask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TaskStartDate",
                table: "Tasks",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskStartDate",
                table: "Tasks");
        }
    }
}
