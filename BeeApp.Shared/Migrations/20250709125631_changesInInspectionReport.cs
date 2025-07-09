using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class changesInInspectionReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignsOfDisease",
                table: "InspectionReports",
                newName: "QueenSeen");

            migrationBuilder.RenameColumn(
                name: "QueenPresent",
                table: "InspectionReports",
                newName: "PollenPresent");

            migrationBuilder.RenameColumn(
                name: "HiveClean",
                table: "InspectionReports",
                newName: "HoneyPresent");

            migrationBuilder.RenameColumn(
                name: "BroodPatternGood",
                table: "InspectionReports",
                newName: "EggsPresent");

            migrationBuilder.RenameColumn(
                name: "AdequateFood",
                table: "InspectionReports",
                newName: "BroodPresent");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "InspectionReports",
                newName: "InspectionReportId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "InspectionReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QueenSeen",
                table: "InspectionReports",
                newName: "SignsOfDisease");

            migrationBuilder.RenameColumn(
                name: "PollenPresent",
                table: "InspectionReports",
                newName: "QueenPresent");

            migrationBuilder.RenameColumn(
                name: "HoneyPresent",
                table: "InspectionReports",
                newName: "HiveClean");

            migrationBuilder.RenameColumn(
                name: "EggsPresent",
                table: "InspectionReports",
                newName: "BroodPatternGood");

            migrationBuilder.RenameColumn(
                name: "BroodPresent",
                table: "InspectionReports",
                newName: "AdequateFood");

            migrationBuilder.RenameColumn(
                name: "InspectionReportId",
                table: "InspectionReports",
                newName: "ReportId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "InspectionReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
