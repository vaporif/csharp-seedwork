using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    public partial class ReworkDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DivisionId",
                table: "Users",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Divisions_DivisionId",
                table: "Users",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Divisions_DivisionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DivisionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Users");
        }
    }
}
