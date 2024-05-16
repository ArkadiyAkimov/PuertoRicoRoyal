using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuertoRicoAPI.Migrations
{
    /// <inheritdoc />
    public partial class hitlerIsGod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "Players",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "Players",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
