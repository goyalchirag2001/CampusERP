using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnyRemainingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceCorrectionRequests_AttendanceRecord_AttendanceRecordId",
                table: "AttendanceCorrectionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_AttendanceSession_AttendanceSessionId",
                table: "AttendanceRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_Students_StudentId",
                table: "AttendanceRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_Users_MarkedByUserId",
                table: "AttendanceRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_AcademicSessions_AcademicSessionId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Campuses_CampusId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Institutions_InstitutionId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_LectureOverrides_LectureOverrideId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Rooms_RoomId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Sections_SectionId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_SemesterSubjects_SemesterSubjectId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Subjects_SubjectId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_TeacherAssignments_TeacherAssignmentId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Teachers_TeacherId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_TimetableTemplates_TimetableTemplateId",
                table: "AttendanceSession");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceSession_Users_LockedByUserId",
                table: "AttendanceSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceSession",
                table: "AttendanceSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecord",
                table: "AttendanceRecord");

            migrationBuilder.RenameTable(
                name: "AttendanceSession",
                newName: "AttendanceSessions");

            migrationBuilder.RenameTable(
                name: "AttendanceRecord",
                newName: "AttendanceRecords");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_TimetableTemplateId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_TimetableTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_TeacherId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_TeacherAssignmentId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_TeacherAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_SubjectId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_SemesterSubjectId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_SemesterSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_SectionId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_RoomId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_LockedByUserId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_LockedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_LectureOverrideId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_LectureOverrideId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_InstitutionId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_InstitutionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_CampusId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_CampusId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSession_AcademicSessionId",
                table: "AttendanceSessions",
                newName: "IX_AttendanceSessions_AcademicSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_StudentId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_MarkedByUserId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_MarkedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_AttendanceSessionId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_AttendanceSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceSessions",
                table: "AttendanceSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceCorrectionRequests_AttendanceRecords_AttendanceRecordId",
                table: "AttendanceCorrectionRequests",
                column: "AttendanceRecordId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceCorrectionRequests_AttendanceRecords_AttendanceRecordId",
                table: "AttendanceCorrectionRequests");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceSessions",
                table: "AttendanceSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords");

            migrationBuilder.RenameTable(
                name: "AttendanceSessions",
                newName: "AttendanceSession");

            migrationBuilder.RenameTable(
                name: "AttendanceRecords",
                newName: "AttendanceRecord");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_TimetableTemplateId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_TimetableTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_TeacherId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_TeacherAssignmentId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_TeacherAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_SubjectId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_SemesterSubjectId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_SemesterSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_SectionId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_RoomId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_LockedByUserId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_LockedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_LectureOverrideId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_LectureOverrideId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_InstitutionId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_InstitutionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_CampusId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_CampusId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceSessions_AcademicSessionId",
                table: "AttendanceSession",
                newName: "IX_AttendanceSession_AcademicSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_StudentId",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_MarkedByUserId",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_MarkedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_AttendanceSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceSession",
                table: "AttendanceSession",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecord",
                table: "AttendanceRecord",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceCorrectionRequests_AttendanceRecord_AttendanceRecordId",
                table: "AttendanceCorrectionRequests",
                column: "AttendanceRecordId",
                principalTable: "AttendanceRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_AttendanceSession_AttendanceSessionId",
                table: "AttendanceRecord",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_Students_StudentId",
                table: "AttendanceRecord",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_Users_MarkedByUserId",
                table: "AttendanceRecord",
                column: "MarkedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_AcademicSessions_AcademicSessionId",
                table: "AttendanceSession",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Campuses_CampusId",
                table: "AttendanceSession",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Institutions_InstitutionId",
                table: "AttendanceSession",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_LectureOverrides_LectureOverrideId",
                table: "AttendanceSession",
                column: "LectureOverrideId",
                principalTable: "LectureOverrides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Rooms_RoomId",
                table: "AttendanceSession",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Sections_SectionId",
                table: "AttendanceSession",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_SemesterSubjects_SemesterSubjectId",
                table: "AttendanceSession",
                column: "SemesterSubjectId",
                principalTable: "SemesterSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Subjects_SubjectId",
                table: "AttendanceSession",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_TeacherAssignments_TeacherAssignmentId",
                table: "AttendanceSession",
                column: "TeacherAssignmentId",
                principalTable: "TeacherAssignments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Teachers_TeacherId",
                table: "AttendanceSession",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_TimetableTemplates_TimetableTemplateId",
                table: "AttendanceSession",
                column: "TimetableTemplateId",
                principalTable: "TimetableTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceSession_Users_LockedByUserId",
                table: "AttendanceSession",
                column: "LockedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
