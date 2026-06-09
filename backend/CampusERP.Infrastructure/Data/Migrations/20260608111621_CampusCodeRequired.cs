using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampusCodeRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Campuses_InstitutionId_Name",
                table: "Campuses");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Campuses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_InstitutionId_Code",
                table: "Campuses",
                columns: new[] { "InstitutionId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Campuses_InstitutionId_Code",
                table: "Campuses");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Campuses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_InstitutionId_Name",
                table: "Campuses",
                columns: new[] { "InstitutionId", "Name" },
                unique: true);
        }
    }
}
