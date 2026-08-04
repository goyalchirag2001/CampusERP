using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCalendarEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_AcademicSessionId_EventType",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_CampusId",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_RoomId_StartDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_SectionId_StartDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_TeacherId_StartDate",
                table: "CalendarEvents");

            migrationBuilder.RenameColumn(
                name: "IsAllDay",
                table: "CalendarEvents",
                newName: "IsRecurring");

            migrationBuilder.AddColumn<bool>(
                name: "AffectsTimetable",
                table: "CalendarEvents",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CalendarEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "CalendarEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFullDay",
                table: "CalendarEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "CalendarEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceRule",
                table: "CalendarEvents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SemesterId",
                table: "CalendarEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_AcademicSessionId_StartDate_EndDate",
                table: "CalendarEvents",
                columns: new[] { "AcademicSessionId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CampusId_StartDate_EventType_IsActive",
                table: "CalendarEvents",
                columns: new[] { "CampusId", "StartDate", "EventType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CourseId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "CourseId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_DepartmentId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "DepartmentId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_EventType_StartDate",
                table: "CalendarEvents",
                columns: new[] { "EventType", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_RoomId_StartDate_IsActive",
                table: "CalendarEvents",
                columns: new[] { "RoomId", "StartDate", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SectionId_StartDate_IsActive",
                table: "CalendarEvents",
                columns: new[] { "SectionId", "StartDate", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SemesterId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "SemesterId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_TeacherId_StartDate_IsActive",
                table: "CalendarEvents",
                columns: new[] { "TeacherId", "StartDate", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_Courses_CourseId",
                table: "CalendarEvents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_Departments_DepartmentId",
                table: "CalendarEvents",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_Semesters_SemesterId",
                table: "CalendarEvents",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_Courses_CourseId",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_Departments_DepartmentId",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_Semesters_SemesterId",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_AcademicSessionId_StartDate_EndDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_CampusId_StartDate_EventType_IsActive",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_CourseId_StartDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_DepartmentId_StartDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_EventType_StartDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_RoomId_StartDate_IsActive",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_SectionId_StartDate_IsActive",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_SemesterId_StartDate",
                table: "CalendarEvents");

            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_TeacherId_StartDate_IsActive",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "AffectsTimetable",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "IsFullDay",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "RecurrenceRule",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "CalendarEvents");

            migrationBuilder.RenameColumn(
                name: "IsRecurring",
                table: "CalendarEvents",
                newName: "IsAllDay");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_AcademicSessionId_EventType",
                table: "CalendarEvents",
                columns: new[] { "AcademicSessionId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CampusId",
                table: "CalendarEvents",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_RoomId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "RoomId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SectionId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "SectionId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_TeacherId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "TeacherId", "StartDate" });
        }
    }
}
