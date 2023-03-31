using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class UpdateTeamReg4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TeamRegistrations",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TeamRegistrations");
        }
    }
}
