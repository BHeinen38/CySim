using Microsoft.EntityFrameworkCore.Migrations;

namespace CySim.Migrations
{
    public partial class changinguserpropertyinteamregistrationmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TeamRegistrations_TeamRegistrationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TeamRegistrationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamRegistrationId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamRegistrationId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TeamRegistrationId",
                table: "AspNetUsers",
                column: "TeamRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TeamRegistrations_TeamRegistrationId",
                table: "AspNetUsers",
                column: "TeamRegistrationId",
                principalTable: "TeamRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
