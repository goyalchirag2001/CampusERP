using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Users_MarkedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_AcademicSessions_AcademicSessionId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Campuses_CampusId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Institutions_InstitutionId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_LectureOverrides_LectureOverrideId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Rooms_RoomId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Sections_SectionId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_SemesterSubjects_SemesterSubjectId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Subjects_SubjectId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_TeacherAssignments_TeacherAssignmentId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Teachers_TeacherId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_TimetableTemplates_TimetableTemplateId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Users_LockedByUserId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_AcademicSessionId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_InstitutionId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_LectureOverrideId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_SectionId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_TeacherId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_TimetableTemplateId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "AttendanceSessions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLocked",
                table: "AttendanceSessions",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAttendanceMarked",
                table: "AttendanceSessions",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMarked",
                table: "AttendanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "MarkingMethod",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_AcademicSessionId_SectionId_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "AcademicSessionId", "SectionId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_InstitutionId_CampusId_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "InstitutionId", "CampusId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_LectureOverrideId_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "LectureOverrideId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_SectionId_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "SectionId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_Status_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "Status", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_TeacherId_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "TeacherId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_TimetableTemplateId_AttendanceDate",
                table: "AttendanceSessions",
                columns: new[] { "TimetableTemplateId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId_StudentId",
                table: "AttendanceRecords",
                columns: new[] { "AttendanceSessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_InstitutionId_CampusId_StudentId",
                table: "AttendanceRecords",
                columns: new[] { "InstitutionId", "CampusId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_IsMarked",
                table: "AttendanceRecords",
                column: "IsMarked");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MarkedOn",
                table: "AttendanceRecords",
                column: "MarkedOn");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MarkingMethod",
                table: "AttendanceRecords",
                column: "MarkingMethod");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_Status",
                table: "AttendanceRecords",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Users_MarkedByUserId",
                table: "AttendanceRecords",
                column: "MarkedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_AcademicSessions_AcademicSessionId",
                table: "AttendanceSessions",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Campuses_CampusId",
                table: "AttendanceSessions",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Institutions_InstitutionId",
                table: "AttendanceSessions",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_LectureOverrides_LectureOverrideId",
                table: "AttendanceSessions",
                column: "LectureOverrideId",
                principalTable: "LectureOverrides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Rooms_RoomId",
                table: "AttendanceSessions",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Sections_SectionId",
                table: "AttendanceSessions",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_SemesterSubjects_SemesterSubjectId",
                table: "AttendanceSessions",
                column: "SemesterSubjectId",
                principalTable: "SemesterSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Subjects_SubjectId",
                table: "AttendanceSessions",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_TeacherAssignments_TeacherAssignmentId",
                table: "AttendanceSessions",
                column: "TeacherAssignmentId",
                principalTable: "TeacherAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Teachers_TeacherId",
                table: "AttendanceSessions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_TimetableTemplates_TimetableTemplateId",
                table: "AttendanceSessions",
                column: "TimetableTemplateId",
                principalTable: "TimetableTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Users_LockedByUserId",
                table: "AttendanceSessions",
                column: "LockedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Users_MarkedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_AcademicSessions_AcademicSessionId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Campuses_CampusId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Institutions_InstitutionId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_LectureOverrides_LectureOverrideId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Rooms_RoomId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Sections_SectionId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_SemesterSubjects_SemesterSubjectId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Subjects_SubjectId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_TeacherAssignments_TeacherAssignmentId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Teachers_TeacherId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_TimetableTemplates_TimetableTemplateId",
                table: "AttendanceSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSessions_Users_LockedByUserId",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_AcademicSessionId_SectionId_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_InstitutionId_CampusId_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_LectureOverrideId_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_SectionId_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_Status_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_TeacherId_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_TimetableTemplateId_AttendanceDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_InstitutionId_CampusId_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_IsMarked",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_MarkedOn",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_MarkingMethod",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_Status",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "MarkingMethod",
                table: "AttendanceRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "AttendanceSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLocked",
                table: "AttendanceSessions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAttendanceMarked",
                table: "AttendanceSessions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMarked",
                table: "AttendanceRecords",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_AcademicSessionId",
                table: "AttendanceSessions",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_InstitutionId",
                table: "AttendanceSessions",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_LectureOverrideId",
                table: "AttendanceSessions",
                column: "LectureOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_SectionId",
                table: "AttendanceSessions",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_TeacherId",
                table: "AttendanceSessions",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_TimetableTemplateId",
                table: "AttendanceSessions",
                column: "TimetableTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId",
                table: "AttendanceRecords",
                column: "AttendanceSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Students_StudentId",
                table: "AttendanceRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Users_MarkedByUserId",
                table: "AttendanceRecords",
                column: "MarkedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_AcademicSessions_AcademicSessionId",
                table: "AttendanceSessions",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Campuses_CampusId",
                table: "AttendanceSessions",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Institutions_InstitutionId",
                table: "AttendanceSessions",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_LectureOverrides_LectureOverrideId",
                table: "AttendanceSessions",
                column: "LectureOverrideId",
                principalTable: "LectureOverrides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Rooms_RoomId",
                table: "AttendanceSessions",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Sections_SectionId",
                table: "AttendanceSessions",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_SemesterSubjects_SemesterSubjectId",
                table: "AttendanceSessions",
                column: "SemesterSubjectId",
                principalTable: "SemesterSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Subjects_SubjectId",
                table: "AttendanceSessions",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_TeacherAssignments_TeacherAssignmentId",
                table: "AttendanceSessions",
                column: "TeacherAssignmentId",
                principalTable: "TeacherAssignments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Teachers_TeacherId",
                table: "AttendanceSessions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_TimetableTemplates_TimetableTemplateId",
                table: "AttendanceSessions",
                column: "TimetableTemplateId",
                principalTable: "TimetableTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSessions_Users_LockedByUserId",
                table: "AttendanceSessions",
                column: "LockedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
