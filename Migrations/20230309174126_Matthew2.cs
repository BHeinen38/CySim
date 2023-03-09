using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class Matthew2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Flags",
                table: "ScoreBoards",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ScoreBoards");
        }
    }
}
