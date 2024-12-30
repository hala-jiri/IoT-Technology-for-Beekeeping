using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiaryDataCollector.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber",
                table: "Hives");

            migrationBuilder.DropTable(
                name: "SensorData");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Apiaries");

            migrationBuilder.DropColumn(
                name: "LightIntensity",
                table: "Apiaries");

            migrationBuilder.DropColumn(
                name: "ReportDate",
                table: "Apiaries");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Apiaries");

            migrationBuilder.AlterColumn<int>(
                name: "ApiaryNumber",
                table: "Hives",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Hives",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Apiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiaryMeasurement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiaryNumber = table.Column<int>(type: "int", nullable: false),
                    MeasurementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<int>(type: "int", nullable: false),
                    LightIntensity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiaryMeasurement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiaryMeasurement_Apiaries_ApiaryNumber",
                        column: x => x.ApiaryNumber,
                        principalTable: "Apiaries",
                        principalColumn: "ApiaryNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HiveMeasurement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HiveNumber = table.Column<int>(type: "int", nullable: false),
                    MeasurementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiveMeasurement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiveMeasurement_Hives_HiveNumber",
                        column: x => x.HiveNumber,
                        principalTable: "Hives",
                        principalColumn: "HiveNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionReport",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QueenPresent = table.Column<bool>(type: "bit", nullable: true),
                    SignsOfDisease = table.Column<bool>(type: "bit", nullable: true),
                    AdequateFood = table.Column<bool>(type: "bit", nullable: true),
                    HiveClean = table.Column<bool>(type: "bit", nullable: true),
                    BroodPatternGood = table.Column<bool>(type: "bit", nullable: true),
                    HiveNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReport", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_InspectionReport_Hives_HiveNumber",
                        column: x => x.HiveNumber,
                        principalTable: "Hives",
                        principalColumn: "HiveNumber");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiaryMeasurement_ApiaryNumber",
                table: "ApiaryMeasurement",
                column: "ApiaryNumber");

            migrationBuilder.CreateIndex(
                name: "IX_HiveMeasurement_HiveNumber",
                table: "HiveMeasurement",
                column: "HiveNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReport_HiveNumber",
                table: "InspectionReport",
                column: "HiveNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber",
                table: "Hives",
                column: "ApiaryNumber",
                principalTable: "Apiaries",
                principalColumn: "ApiaryNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber",
                table: "Hives");

            migrationBuilder.DropTable(
                name: "ApiaryMeasurement");

            migrationBuilder.DropTable(
                name: "HiveMeasurement");

            migrationBuilder.DropTable(
                name: "InspectionReport");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Hives");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Apiaries");

            migrationBuilder.AlterColumn<int>(
                name: "ApiaryNumber",
                table: "Hives",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "Apiaries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LightIntensity",
                table: "Apiaries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportDate",
                table: "Apiaries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Temperature",
                table: "Apiaries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SensorData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HiveNumber1 = table.Column<int>(type: "int", nullable: false),
                    HiveNumber = table.Column<int>(type: "int", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorData_Hives_HiveNumber1",
                        column: x => x.HiveNumber1,
                        principalTable: "Hives",
                        principalColumn: "HiveNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_HiveNumber1",
                table: "SensorData",
                column: "HiveNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber",
                table: "Hives",
                column: "ApiaryNumber",
                principalTable: "Apiaries",
                principalColumn: "ApiaryNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
