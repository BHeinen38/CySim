using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class Tutorialupdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isRed",
                table: "Tutorials",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isRed",
                table: "Tutorials");
        }
    }
}
