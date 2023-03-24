using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class Scoreboard1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Flags",
                table: "ScoreBoards",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Usability",
                table: "ScoreBoards",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ScoreBoards");

            migrationBuilder.DropColumn(
                name: "Usability",
                table: "ScoreBoards");
        }
    }
}
