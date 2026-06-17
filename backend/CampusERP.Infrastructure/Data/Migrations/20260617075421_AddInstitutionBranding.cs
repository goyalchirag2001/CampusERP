using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionBranding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginSlug",
                table: "Institutions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Institutions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Institutions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "Institutions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_LoginSlug",
                table: "Institutions",
                column: "LoginSlug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Institutions_LoginSlug",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "LoginSlug",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "Institutions");
        }
    }
}
