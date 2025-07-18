using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddApiaryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Apiaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Apiaries");
        }
    }
}
