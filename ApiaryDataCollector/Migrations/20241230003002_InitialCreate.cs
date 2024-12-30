using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiaryDataCollector.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apiaries",
                columns: table => new
                {
                    ApiaryNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Humidity = table.Column<int>(type: "int", nullable: true),
                    Temperature = table.Column<int>(type: "int", nullable: true),
                    LightIntensity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apiaries", x => x.ApiaryNumber);
                });

            migrationBuilder.CreateTable(
                name: "Hives",
                columns: table => new
                {
                    HiveNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiaryNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hives", x => x.HiveNumber);
                    table.ForeignKey(
                        name: "FK_Hives_Apiaries_ApiaryNumber",
                        column: x => x.ApiaryNumber,
                        principalTable: "Apiaries",
                        principalColumn: "ApiaryNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    HiveNumber = table.Column<int>(type: "int", nullable: false),
                    HiveNumber1 = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Hives_ApiaryNumber",
                table: "Hives",
                column: "ApiaryNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_HiveNumber1",
                table: "SensorData",
                column: "HiveNumber1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorData");

            migrationBuilder.DropTable(
                name: "Hives");

            migrationBuilder.DropTable(
                name: "Apiaries");
        }
    }
}
