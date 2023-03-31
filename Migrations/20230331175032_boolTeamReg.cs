using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class boolTeamReg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRed",
                table: "TeamRegistrations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRed",
                table: "TeamRegistrations");
        }
    }
}
