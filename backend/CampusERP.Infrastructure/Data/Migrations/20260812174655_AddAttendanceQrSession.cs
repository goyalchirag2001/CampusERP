using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceQrSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceQrSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ClosedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceQrSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceQrSessions_AttendanceSessions_AttendanceSessionId",
                        column: x => x.AttendanceSessionId,
                        principalTable: "AttendanceSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceQrSessions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_AttendanceSessionId_IsActive",
                table: "AttendanceQrSessions",
                columns: new[] { "AttendanceSessionId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_CreatedByUserId",
                table: "AttendanceQrSessions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_ExpiresOn_IsActive",
                table: "AttendanceQrSessions",
                columns: new[] { "ExpiresOn", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_InstitutionId_CampusId_AttendanceSessionId",
                table: "AttendanceQrSessions",
                columns: new[] { "InstitutionId", "CampusId", "AttendanceSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_Token",
                table: "AttendanceQrSessions",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceQrSessions");
        }
    }
}
