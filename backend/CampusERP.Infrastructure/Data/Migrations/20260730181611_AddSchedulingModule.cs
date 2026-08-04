using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedulingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoomNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HasProjector = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasSmartBoard = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasAirConditioning = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasComputers = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasInternet = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAccessible = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcademicSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_AcademicSessions_AcademicSessionId",
                        column: x => x.AcademicSessionId,
                        principalTable: "AcademicSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimetableTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: false),
                    LectureType = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableTemplates_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableTemplates_TeacherAssignments_TeacherAssignmentId",
                        column: x => x.TeacherAssignmentId,
                        principalTable: "TeacherAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LectureOverrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimetableTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalendarEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewTeacherAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OccurrenceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OverrideType = table.Column<int>(type: "int", nullable: false),
                    NewStartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    NewEndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LectureOverrides_CalendarEvents_CalendarEventId",
                        column: x => x.CalendarEventId,
                        principalTable: "CalendarEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LectureOverrides_Rooms_NewRoomId",
                        column: x => x.NewRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LectureOverrides_TeacherAssignments_NewTeacherAssignmentId",
                        column: x => x.NewTeacherAssignmentId,
                        principalTable: "TeacherAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LectureOverrides_TimetableTemplates_TimetableTemplateId",
                        column: x => x.TimetableTemplateId,
                        principalTable: "TimetableTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_AcademicSessionId_EventType",
                table: "CalendarEvents",
                columns: new[] { "AcademicSessionId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CampusId",
                table: "CalendarEvents",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_InstitutionId_CampusId_StartDate_EndDate",
                table: "CalendarEvents",
                columns: new[] { "InstitutionId", "CampusId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_IsActive",
                table: "CalendarEvents",
                column: "IsActive");

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

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_CalendarEventId",
                table: "LectureOverrides",
                column: "CalendarEventId");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_IsActive",
                table: "LectureOverrides",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LectureOverrides_IsApproved",
                table: "LectureOverrides",
                column: "IsApproved");

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

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CampusId_Building_Floor",
                table: "Rooms",
                columns: new[] { "CampusId", "Building", "Floor" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CampusId_RoomType",
                table: "Rooms",
                columns: new[] { "CampusId", "RoomType" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_InstitutionId_CampusId_Building_RoomNumber",
                table: "Rooms",
                columns: new[] { "InstitutionId", "CampusId", "Building", "RoomNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_InstitutionId_CampusId_RoomName",
                table: "Rooms",
                columns: new[] { "InstitutionId", "CampusId", "RoomName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IsActive",
                table: "Rooms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_DayOfWeek_DisplayOrder",
                table: "TimetableTemplates",
                columns: new[] { "DayOfWeek", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_EffectiveFrom_EffectiveTo",
                table: "TimetableTemplates",
                columns: new[] { "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_IsActive",
                table: "TimetableTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_RoomId_DayOfWeek_StartTime_EffectiveFrom_EffectiveTo",
                table: "TimetableTemplates",
                columns: new[] { "RoomId", "DayOfWeek", "StartTime", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableTemplates_TeacherAssignmentId_DayOfWeek_StartTime_EffectiveFrom_EffectiveTo",
                table: "TimetableTemplates",
                columns: new[] { "TeacherAssignmentId", "DayOfWeek", "StartTime", "EffectiveFrom", "EffectiveTo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LectureOverrides");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "TimetableTemplates");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
