using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class UpdateTeamReg3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "TeamRegistrations");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "TeamRegistrations",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "TeamRegistrations");

            migrationBuilder.AddColumn<bool>(
                name: "ProfilePicture",
                table: "TeamRegistrations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
