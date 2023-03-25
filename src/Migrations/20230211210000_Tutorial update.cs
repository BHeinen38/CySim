using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class Tutorialupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Tutorials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Tutorials");
        }
    }
}
