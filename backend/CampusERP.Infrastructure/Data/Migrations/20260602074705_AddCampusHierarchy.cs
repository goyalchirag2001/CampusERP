using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_InstitutionId_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_InstitutionId_EmployeeCode",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_InstitutionId_RollNumber",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Departments_InstitutionId_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_InstitutionId_Name",
                table: "Courses");

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "Teachers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campuses_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CampusId_Email",
                table: "Users",
                columns: new[] { "CampusId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_InstitutionId",
                table: "Users",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_CampusId_EmployeeCode",
                table: "Teachers",
                columns: new[] { "CampusId", "EmployeeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_InstitutionId",
                table: "Teachers",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CampusId_RollNumber",
                table: "Students",
                columns: new[] { "CampusId", "RollNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_InstitutionId",
                table: "Students",
                column: "InstitutionId");

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
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_InstitutionId_Name",
                table: "Campuses",
                columns: new[] { "InstitutionId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Campuses_CampusId",
                table: "Courses",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Campuses_CampusId",
                table: "Departments",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Campuses_CampusId",
                table: "Students",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Campuses_CampusId",
                table: "Teachers",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Campuses_CampusId",
                table: "Users",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Campuses_CampusId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Campuses_CampusId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Campuses_CampusId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Campuses_CampusId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Campuses_CampusId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Campuses");

            migrationBuilder.DropIndex(
                name: "IX_Users_CampusId_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_InstitutionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_CampusId_EmployeeCode",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_InstitutionId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_CampusId_RollNumber",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_InstitutionId",
                table: "Students");

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
                name: "IX_Courses_DepartmentId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InstitutionId_Email",
                table: "Users",
                columns: new[] { "InstitutionId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_InstitutionId_EmployeeCode",
                table: "Teachers",
                columns: new[] { "InstitutionId", "EmployeeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_InstitutionId_RollNumber",
                table: "Students",
                columns: new[] { "InstitutionId", "RollNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_InstitutionId_Name",
                table: "Departments",
                columns: new[] { "InstitutionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId_Name",
                table: "Courses",
                columns: new[] { "InstitutionId", "Name" },
                unique: true);
        }
    }
}
