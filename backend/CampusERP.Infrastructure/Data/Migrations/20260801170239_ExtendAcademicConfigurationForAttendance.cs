using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendAcademicConfigurationForAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_TeacherAssignments_NewTeacherAssignmentId",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_IsActive",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_NewRoomId",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_NewTeacherAssignmentId",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_OverrideType",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_TimetableTemplateId_OccurrenceDate",
                table: "LectureOverrides");

            migrationBuilder.RenameColumn(
                name: "EffectiveTo",
                table: "TimetableTemplates",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "EffectiveFrom",
                table: "TimetableTemplates",
                newName: "ValidFrom");

            migrationBuilder.RenameIndex(
                name: "IX_TimetableTemplates_TeacherAssignmentId_DayOfWeek_StartTime_EffectiveFrom_EffectiveTo",
                table: "TimetableTemplates",
                newName: "IX_TimetableTemplates_TeacherAssignmentId_DayOfWeek_StartTime_ValidFrom_ValidTo");

            migrationBuilder.RenameIndex(
                name: "IX_TimetableTemplates_RoomId_DayOfWeek_StartTime_EffectiveFrom_EffectiveTo",
                table: "TimetableTemplates",
                newName: "IX_TimetableTemplates_RoomId_DayOfWeek_StartTime_ValidFrom_ValidTo");

            migrationBuilder.RenameIndex(
                name: "IX_TimetableTemplates_EffectiveFrom_EffectiveTo",
                table: "TimetableTemplates",
                newName: "IX_TimetableTemplates_ValidFrom_ValidTo");

            migrationBuilder.RenameColumn(
                name: "OccurrenceDate",
                table: "LectureOverrides",
                newName: "OverrideDate");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "LectureOverrides",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "NewTeacherAssignmentId",
                table: "LectureOverrides",
                newName: "OriginalTeacherId");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "LectureOverrides",
                newName: "GenerateAttendance");

            migrationBuilder.AddColumn<Guid>(
                name: "AcademicSessionId",
                table: "TimetableTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "TimetableTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "GenerateAttendance",
                table: "TimetableTemplates",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "TimetableTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "TimetableTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "TimetableTemplates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TimetableTemplates",
                type: "int",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "TimetableTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SemesterSubjectId",
                table: "TimetableTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "TimetableTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "TimetableTemplateId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LectureOverrides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AcademicSessionId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedByUserId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "LectureOverrides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CampusId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemGenerated",
                table: "LectureOverrides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "NewTeacherId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OriginalEndTime",
                table: "LectureOverrides",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalRoomId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OriginalStartTime",
                table: "LectureOverrides",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "LectureOverrides",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "AcademicSessionId",
                table: "CalendarEvents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowStudentAttendanceCorrection",
                table: "AcademicConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowTeacherAttendanceUnlock",
                table: "AcademicConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AttendanceLockAfterDays",
                table: "AcademicConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 7);

            migrationBuilder.AddColumn<bool>(
                name: "AutoGenerateAttendanceRecords",
                table: "AcademicConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AutoGenerateAttendanceSessions",
                table: "AcademicConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "LateThresholdMinutes",
                table: "AcademicConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<bool>(
                name: "MedicalLeaveCountsAsPresent",
                table: "AcademicConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnDutyCountsAsPresent",
                table: "AcademicConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_AcademicSessionId_DayOfWeek",
                table: "TimetableTemplates",
                columns: new[] { "AcademicSessionId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_CampusId",
                table: "TimetableTemplates",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_InstitutionId",
                table: "TimetableTemplates",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_SectionId_DayOfWeek_StartTime_ValidFrom_ValidTo",
                table: "TimetableTemplates",
                columns: new[] { "SectionId", "DayOfWeek", "StartTime", "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_SemesterSubjectId",
                table: "TimetableTemplates",
                column: "SemesterSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_TeacherId_DayOfWeek_StartTime_ValidFrom_ValidTo",
                table: "TimetableTemplates",
                columns: new[] { "TeacherId", "DayOfWeek", "StartTime", "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_SectionId_AcademicSessionId",
                table: "TeacherAssignments",
                columns: new[] { "SectionId", "AcademicSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_SemesterSubjectId_AcademicSessionId",
                table: "TeacherAssignments",
                columns: new[] { "SemesterSubjectId", "AcademicSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TeacherId_AcademicSessionId",
                table: "TeacherAssignments",
                columns: new[] { "TeacherId", "AcademicSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignments_TeacherId_SectionId",
                table: "TeacherAssignments",
                columns: new[] { "TeacherId", "SectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_AcademicSessionId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "AcademicSessionId", "OverrideDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_AcademicSessionId_OverrideDate_OverrideType",
                table: "LectureOverrides",
                columns: new[] { "AcademicSessionId", "OverrideDate", "OverrideType" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_ApprovedByUserId",
                table: "LectureOverrides",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_CampusId",
                table: "LectureOverrides",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_InstitutionId_CampusId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "InstitutionId", "CampusId", "OverrideDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_IsSystemGenerated",
                table: "LectureOverrides",
                column: "IsSystemGenerated");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_NewRoomId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "NewRoomId", "OverrideDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_NewTeacherId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "NewTeacherId", "OverrideDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_OriginalRoomId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "OriginalRoomId", "OverrideDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_OriginalTeacherId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "OriginalTeacherId", "OverrideDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_TimetableTemplateId_OverrideDate",
                table: "LectureOverrides",
                columns: new[] { "TimetableTemplateId", "OverrideDate" },
                unique: true,
                filter: "[TimetableTemplateId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_AcademicSessions_AcademicSessionId",
                table: "LectureOverrides",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_Campuses_CampusId",
                table: "LectureOverrides",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_Institutions_InstitutionId",
                table: "LectureOverrides",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_Rooms_OriginalRoomId",
                table: "LectureOverrides",
                column: "OriginalRoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_Teachers_NewTeacherId",
                table: "LectureOverrides",
                column: "NewTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_Teachers_OriginalTeacherId",
                table: "LectureOverrides",
                column: "OriginalTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_Users_ApprovedByUserId",
                table: "LectureOverrides",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableTemplates_AcademicSessions_AcademicSessionId",
                table: "TimetableTemplates",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableTemplates_Campuses_CampusId",
                table: "TimetableTemplates",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableTemplates_Institutions_InstitutionId",
                table: "TimetableTemplates",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableTemplates_Sections_SectionId",
                table: "TimetableTemplates",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableTemplates_SemesterSubjects_SemesterSubjectId",
                table: "TimetableTemplates",
                column: "SemesterSubjectId",
                principalTable: "SemesterSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableTemplates_Teachers_TeacherId",
                table: "TimetableTemplates",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_AcademicSessions_AcademicSessionId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_Campuses_CampusId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_Institutions_InstitutionId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_Rooms_OriginalRoomId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_Teachers_NewTeacherId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_Teachers_OriginalTeacherId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureOverrides_Users_ApprovedByUserId",
                table: "LectureOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_TimetableTemplates_AcademicSessions_AcademicSessionId",
                table: "TimetableTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimetableTemplates_Campuses_CampusId",
                table: "TimetableTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimetableTemplates_Institutions_InstitutionId",
                table: "TimetableTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimetableTemplates_Sections_SectionId",
                table: "TimetableTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimetableTemplates_SemesterSubjects_SemesterSubjectId",
                table: "TimetableTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimetableTemplates_Teachers_TeacherId",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TimetableTemplates_AcademicSessionId_DayOfWeek",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TimetableTemplates_CampusId",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TimetableTemplates_InstitutionId",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TimetableTemplates_SectionId_DayOfWeek_StartTime_ValidFrom_ValidTo",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TimetableTemplates_SemesterSubjectId",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TimetableTemplates_TeacherId_DayOfWeek_StartTime_ValidFrom_ValidTo",
                table: "TimetableTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_SectionId_AcademicSessionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_SemesterSubjectId_AcademicSessionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_TeacherId_AcademicSessionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAssignments_TeacherId_SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_AcademicSessionId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_AcademicSessionId_OverrideDate_OverrideType",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_ApprovedByUserId",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_CampusId",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_InstitutionId_CampusId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_IsSystemGenerated",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_NewRoomId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_NewTeacherId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_OriginalRoomId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_OriginalTeacherId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropIndex(
                name: "IX_LectureOverrides_TimetableTemplateId_OverrideDate",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "GenerateAttendance",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "SemesterSubjectId",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "TimetableTemplates");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "IsSystemGenerated",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "NewTeacherId",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "OriginalEndTime",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "OriginalRoomId",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "OriginalStartTime",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "LectureOverrides");

            migrationBuilder.DropColumn(
                name: "AllowStudentAttendanceCorrection",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "AllowTeacherAttendanceUnlock",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "AttendanceLockAfterDays",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "AutoGenerateAttendanceRecords",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "AutoGenerateAttendanceSessions",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "LateThresholdMinutes",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "MedicalLeaveCountsAsPresent",
                table: "AcademicConfigurations");

            migrationBuilder.DropColumn(
                name: "OnDutyCountsAsPresent",
                table: "AcademicConfigurations");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "TimetableTemplates",
                newName: "EffectiveTo");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                table: "TimetableTemplates",
                newName: "EffectiveFrom");

            migrationBuilder.RenameIndex(
                name: "IX_TimetableTemplates_ValidFrom_ValidTo",
                table: "TimetableTemplates",
                newName: "IX_TimetableTemplates_EffectiveFrom_EffectiveTo");

            migrationBuilder.RenameIndex(
                name: "IX_TimetableTemplates_TeacherAssignmentId_DayOfWeek_StartTime_ValidFrom_ValidTo",
                table: "TimetableTemplates",
                newName: "IX_TimetableTemplates_TeacherAssignmentId_DayOfWeek_StartTime_EffectiveFrom_EffectiveTo");

            migrationBuilder.RenameIndex(
                name: "IX_TimetableTemplates_RoomId_DayOfWeek_StartTime_ValidFrom_ValidTo",
                table: "TimetableTemplates",
                newName: "IX_TimetableTemplates_RoomId_DayOfWeek_StartTime_EffectiveFrom_EffectiveTo");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "LectureOverrides",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "OverrideDate",
                table: "LectureOverrides",
                newName: "OccurrenceDate");

            migrationBuilder.RenameColumn(
                name: "OriginalTeacherId",
                table: "LectureOverrides",
                newName: "NewTeacherAssignmentId");

            migrationBuilder.RenameColumn(
                name: "GenerateAttendance",
                table: "LectureOverrides",
                newName: "IsActive");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimetableTemplateId",
                table: "LectureOverrides",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LectureOverrides",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AcademicSessionId",
                table: "CalendarEvents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_IsActive",
                table: "LectureOverrides",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_NewRoomId",
                table: "LectureOverrides",
                column: "NewRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_NewTeacherAssignmentId",
                table: "LectureOverrides",
                column: "NewTeacherAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_OverrideType",
                table: "LectureOverrides",
                column: "OverrideType");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_TimetableTemplateId_OccurrenceDate",
                table: "LectureOverrides",
                columns: new[] { "TimetableTemplateId", "OccurrenceDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureOverrides_TeacherAssignments_NewTeacherAssignmentId",
                table: "LectureOverrides",
                column: "NewTeacherAssignmentId",
                principalTable: "TeacherAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
