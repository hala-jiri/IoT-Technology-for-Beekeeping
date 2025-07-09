using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangesNumberToIdName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HiveNotes_Hives_HiveNumber",
                table: "HiveNotes");

            migrationBuilder.DropColumn(
                name: "HiveNumber",
                table: "WarehouseItemUsages");

            migrationBuilder.RenameColumn(
                name: "HiveNumber",
                table: "HiveNotes",
                newName: "HiveId");

            migrationBuilder.RenameIndex(
                name: "IX_HiveNotes_HiveNumber",
                table: "HiveNotes",
                newName: "IX_HiveNotes_HiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_HiveNotes_Hives_HiveId",
                table: "HiveNotes",
                column: "HiveId",
                principalTable: "Hives",
                principalColumn: "HiveId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HiveNotes_Hives_HiveId",
                table: "HiveNotes");

            migrationBuilder.RenameColumn(
                name: "HiveId",
                table: "HiveNotes",
                newName: "HiveNumber");

            migrationBuilder.RenameIndex(
                name: "IX_HiveNotes_HiveId",
                table: "HiveNotes",
                newName: "IX_HiveNotes_HiveNumber");

            migrationBuilder.AddColumn<int>(
                name: "HiveNumber",
                table: "WarehouseItemUsages",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HiveNotes_Hives_HiveNumber",
                table: "HiveNotes",
                column: "HiveNumber",
                principalTable: "Hives",
                principalColumn: "HiveId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
