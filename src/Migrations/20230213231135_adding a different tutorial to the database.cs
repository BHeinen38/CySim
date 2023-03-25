using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class addingadifferenttutorialtothedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isGameType",
                table: "Tutorials",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isGameType",
                table: "Tutorials");
        }
    }
}
