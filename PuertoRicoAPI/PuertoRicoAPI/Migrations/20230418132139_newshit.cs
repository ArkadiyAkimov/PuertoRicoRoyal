using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuertoRicoAPI.Migrations
{
    /// <inheritdoc />
    public partial class newshit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanUseLargeWarehouse",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanUseSmallWarehouse",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanUseLargeWarehouse",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CanUseSmallWarehouse",
                table: "Players");
        }
    }
}
