using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class NewTutorial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tutorials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tutorials");
        }
    }
}
