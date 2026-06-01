using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_EmployeeCode",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_RollNumber",
                table: "Students");

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Teachers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Code",
                table: "Institutions",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Institutions_InstitutionId",
                table: "Courses",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Institutions_InstitutionId",
                table: "Departments",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Institutions_InstitutionId",
                table: "Students",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Institutions_InstitutionId",
                table: "Teachers",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Institutions_InstitutionId",
                table: "Users",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Institutions_InstitutionId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Institutions_InstitutionId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Institutions_InstitutionId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Institutions_InstitutionId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Institutions_InstitutionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Institutions");

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

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_EmployeeCode",
                table: "Teachers",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_RollNumber",
                table: "Students",
                column: "RollNumber",
                unique: true);
        }
    }
}
