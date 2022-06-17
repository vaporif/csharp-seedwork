using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    public partial class AddDivisionEmployee1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DivisionEmployee_Division_DivisionId",
                table: "DivisionEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionEmployee_Employee_EmployeeId",
                table: "DivisionEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Division",
                table: "Division");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "Division",
                newName: "Divisions");

            migrationBuilder.RenameIndex(
                name: "IX_Division_Name",
                table: "Divisions",
                newName: "IX_Divisions_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionEmployee_Divisions_DivisionId",
                table: "DivisionEmployee",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionEmployee_Employees_EmployeeId",
                table: "DivisionEmployee",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DivisionEmployee_Divisions_DivisionId",
                table: "DivisionEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionEmployee_Employees_EmployeeId",
                table: "DivisionEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employee");

            migrationBuilder.RenameTable(
                name: "Divisions",
                newName: "Division");

            migrationBuilder.RenameIndex(
                name: "IX_Divisions_Name",
                table: "Division",
                newName: "IX_Division_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Division",
                table: "Division",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionEmployee_Division_DivisionId",
                table: "DivisionEmployee",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionEmployee_Employee_EmployeeId",
                table: "DivisionEmployee",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
