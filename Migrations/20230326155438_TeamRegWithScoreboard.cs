using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class TeamRegWithScoreboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Flags",
                table: "TeamRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "TeamRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Usability",
                table: "TeamRegistrations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flags",
                table: "TeamRegistrations");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "TeamRegistrations");

            migrationBuilder.DropColumn(
                name: "Usability",
                table: "TeamRegistrations");
        }
    }
}
