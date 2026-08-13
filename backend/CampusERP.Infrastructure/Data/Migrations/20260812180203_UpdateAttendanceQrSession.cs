using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttendanceQrSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceQrSessions_AttendanceSessionId_IsActive",
                table: "AttendanceQrSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_AttendanceSessionId",
                table: "AttendanceQrSessions",
                column: "AttendanceSessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceQrSessions_AttendanceSessionId",
                table: "AttendanceQrSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_AttendanceSessionId_IsActive",
                table: "AttendanceQrSessions",
                columns: new[] { "AttendanceSessionId", "IsActive" });
        }
    }
}
