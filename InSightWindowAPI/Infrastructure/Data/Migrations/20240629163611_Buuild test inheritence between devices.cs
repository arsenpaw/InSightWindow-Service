using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class Buuildtestinheritencebetweendevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Devices",
                newName: "DeviceType");

            migrationBuilder.AddColumn<int>(
                name: "LightPowered",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isOn",
                table: "Devices",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isOpen",
                table: "Devices",
                type: "bit",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("92381f13-15eb-4ed4-ad57-19b0ef46b57d"), "Window", null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("e02814f3-4673-4cbb-b022-b3a7133b1a22"), "BulbTest", 24, null, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("92381f13-15eb-4ed4-ad57-19b0ef46b57d"));

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("e02814f3-4673-4cbb-b022-b3a7133b1a22"));

            migrationBuilder.DropColumn(
                name: "LightPowered",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isOn",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isOpen",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "DeviceType",
                table: "Devices",
                newName: "Name");
        }
    }
}
