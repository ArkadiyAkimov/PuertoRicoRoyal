using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuertoRicoAPI.Migrations
{
    /// <inheritdoc />
    public partial class hitlerIsTheKing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "GameOver",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LastGovernor",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GameOver",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastGovernor",
                table: "Games");
        }
    }
}
