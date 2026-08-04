using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcademicTermType = table.Column<int>(type: "int", nullable: false),
                    AcademicTermsPerSession = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    AutoPromoteEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinimumAttendancePercentage = table.Column<int>(type: "int", nullable: false, defaultValue: 75),
                    AllowAttendanceEditing = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AttendanceEditWindowDays = table.Column<int>(type: "int", nullable: false, defaultValue: 7),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicConfigurations_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicConfigurations_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicConfigurations_CampusId",
                table: "AcademicConfigurations",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicConfigurations_InstitutionId_CampusId",
                table: "AcademicConfigurations",
                columns: new[] { "InstitutionId", "CampusId" },
                unique: true,
                filter: "[CampusId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicConfigurations");
        }
    }
}
