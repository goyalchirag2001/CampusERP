using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceSession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimetableTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LectureOverrideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LectureType = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsAttendanceMarked = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_AcademicSessions_AcademicSessionId",
                        column: x => x.AcademicSessionId,
                        principalTable: "AcademicSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_LectureOverrides_LectureOverrideId",
                        column: x => x.LectureOverrideId,
                        principalTable: "LectureOverrides",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_SemesterSubjects_SemesterSubjectId",
                        column: x => x.SemesterSubjectId,
                        principalTable: "SemesterSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_TeacherAssignments_TeacherAssignmentId",
                        column: x => x.TeacherAssignmentId,
                        principalTable: "TeacherAssignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSession_TimetableTemplates_TimetableTemplateId",
                        column: x => x.TimetableTemplateId,
                        principalTable: "TimetableTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendanceSession_Users_LockedByUserId",
                        column: x => x.LockedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsMarked = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MarkedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarkedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_AttendanceSession_AttendanceSessionId",
                        column: x => x.AttendanceSessionId,
                        principalTable: "AttendanceSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_Users_MarkedByUserId",
                        column: x => x.MarkedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttendanceCorrectionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewRemarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttendanceUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalStatus = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceCorrectionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_AttendanceRecord_AttendanceRecordId",
                        column: x => x.AttendanceRecordId,
                        principalTable: "AttendanceRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceCorrectionRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_AttendanceRecordId",
                table: "AttendanceCorrectionRequests",
                column: "AttendanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_AttendanceRecordId_Status",
                table: "AttendanceCorrectionRequests",
                columns: new[] { "AttendanceRecordId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_AttendanceRecordId_StudentId",
                table: "AttendanceCorrectionRequests",
                columns: new[] { "AttendanceRecordId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_CreatedAt",
                table: "AttendanceCorrectionRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_InstitutionId_CampusId_Status",
                table: "AttendanceCorrectionRequests",
                columns: new[] { "InstitutionId", "CampusId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_IsProcessed",
                table: "AttendanceCorrectionRequests",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_Reason",
                table: "AttendanceCorrectionRequests",
                column: "Reason");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_ReviewedByUserId",
                table: "AttendanceCorrectionRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_Status",
                table: "AttendanceCorrectionRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_StudentId",
                table: "AttendanceCorrectionRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrectionRequests_StudentId_Status",
                table: "AttendanceCorrectionRequests",
                columns: new[] { "StudentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_AttendanceSessionId",
                table: "AttendanceRecord",
                column: "AttendanceSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_MarkedByUserId",
                table: "AttendanceRecord",
                column: "MarkedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_StudentId",
                table: "AttendanceRecord",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_AcademicSessionId",
                table: "AttendanceSession",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_CampusId",
                table: "AttendanceSession",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_InstitutionId",
                table: "AttendanceSession",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_LectureOverrideId",
                table: "AttendanceSession",
                column: "LectureOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_LockedByUserId",
                table: "AttendanceSession",
                column: "LockedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_RoomId",
                table: "AttendanceSession",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_SectionId",
                table: "AttendanceSession",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_SemesterSubjectId",
                table: "AttendanceSession",
                column: "SemesterSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_SubjectId",
                table: "AttendanceSession",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_TeacherAssignmentId",
                table: "AttendanceSession",
                column: "TeacherAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_TeacherId",
                table: "AttendanceSession",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSession_TimetableTemplateId",
                table: "AttendanceSession",
                column: "TimetableTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceCorrectionRequests");

            migrationBuilder.DropTable(
                name: "AttendanceRecord");

            migrationBuilder.DropTable(
                name: "AttendanceSession");
        }
    }
}
