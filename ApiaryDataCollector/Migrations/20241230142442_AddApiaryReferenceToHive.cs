using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiaryDataCollector.Migrations
{
    /// <inheritdoc />
    public partial class AddApiaryReferenceToHive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber",
                table: "Hives");

            migrationBuilder.DropIndex(
                name: "IX_Hives_ApiaryNumber",
                table: "Hives");

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
                name: "ApiaryNumber1",
                table: "Hives",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hives_ApiaryNumber1",
                table: "Hives",
                column: "ApiaryNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber1",
                table: "Hives",
                column: "ApiaryNumber1",
                principalTable: "Apiaries",
                principalColumn: "ApiaryNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber1",
                table: "Hives");

            migrationBuilder.DropIndex(
                name: "IX_Hives_ApiaryNumber1",
                table: "Hives");

            migrationBuilder.DropColumn(
                name: "ApiaryNumber1",
                table: "Hives");

            migrationBuilder.AlterColumn<int>(
                name: "ApiaryNumber",
                table: "Hives",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Hives_ApiaryNumber",
                table: "Hives",
                column: "ApiaryNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Hives_Apiaries_ApiaryNumber",
                table: "Hives",
                column: "ApiaryNumber",
                principalTable: "Apiaries",
                principalColumn: "ApiaryNumber");
        }
    }
}
