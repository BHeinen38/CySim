using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class Tutorialupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tutorials");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Tutorials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Tutorials");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tutorials",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
