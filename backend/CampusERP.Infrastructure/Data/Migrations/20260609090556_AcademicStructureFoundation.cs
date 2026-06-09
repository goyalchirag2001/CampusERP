using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AcademicStructureFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_CampusId_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_InstitutionId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CampusId_DepartmentId_Name",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Departments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Courses",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentId",
                table: "Students",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CampusId",
                table: "Departments",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_InstitutionId_CampusId_Code",
                table: "Departments",
                columns: new[] { "InstitutionId", "CampusId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CampusId",
                table: "Courses",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId_CampusId_Code",
                table: "Courses",
                columns: new[] { "InstitutionId", "CampusId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departments_DepartmentId",
                table: "Students",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departments_DepartmentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_DepartmentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CampusId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_InstitutionId_CampusId_Code",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CampusId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_InstitutionId_CampusId_Code",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CampusId_Name",
                table: "Departments",
                columns: new[] { "CampusId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_InstitutionId",
                table: "Departments",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CampusId_DepartmentId_Name",
                table: "Courses",
                columns: new[] { "CampusId", "DepartmentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses",
                column: "InstitutionId");
        }
    }
}
