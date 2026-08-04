using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherAssignmentForSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_TeacherId_SemesterSubjectId",
                table: "TeacherAssignments");

            migrationBuilder.AddColumn<Guid>(
                name: "AcademicSessionId",
                table: "TeacherAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "TeacherAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "YearNumber",
                table: "Semesters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_AcademicSessionId",
                table: "TeacherAssignments",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_AcademicSessionId_SectionId_SemesterSubjectId",
                table: "TeacherAssignments",
                columns: new[] { "AcademicSessionId", "SectionId", "SemesterSubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_SectionId",
                table: "TeacherAssignments",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_AcademicSessions_AcademicSessionId",
                table: "TeacherAssignments",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Sections_SectionId",
                table: "TeacherAssignments",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_AcademicSessions_AcademicSessionId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Sections_SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_AcademicSessionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_AcademicSessionId_SectionId_SemesterSubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropColumn(
                name: "YearNumber",
                table: "Semesters");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TeacherId_SemesterSubjectId",
                table: "TeacherAssignments",
                columns: new[] { "TeacherId", "SemesterSubjectId" },
                unique: true);
        }
    }
}
