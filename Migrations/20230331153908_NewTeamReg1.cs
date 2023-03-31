using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class NewTeamReg1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamCreator",
                table: "TeamRegistrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamCreator",
                table: "TeamRegistrations");
        }
    }
}
