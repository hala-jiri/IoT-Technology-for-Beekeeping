using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddApiaryCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Apiaries",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Apiaries",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Apiaries");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Apiaries");
        }
    }
}
