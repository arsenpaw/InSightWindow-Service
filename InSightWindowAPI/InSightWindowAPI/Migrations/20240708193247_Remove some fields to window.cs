using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class Removesomefieldstowindow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Temparature",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isRain",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Temparature",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isRain",
                table: "Devices",
                type: "int",
                nullable: true);
        }
    }
}
