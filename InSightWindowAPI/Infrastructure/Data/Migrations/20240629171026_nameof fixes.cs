using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class nameoffixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("92381f13-15eb-4ed4-ad57-19b0ef46b57d"));

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("e02814f3-4673-4cbb-b022-b3a7133b1a22"));

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("56af4916-703d-40fc-9268-093e6db2d4df"), "Window", null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("7184508c-d179-4e40-acac-e701502f0af8"), "BulbTest", 24, null, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("56af4916-703d-40fc-9268-093e6db2d4df"));

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("7184508c-d179-4e40-acac-e701502f0af8"));

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("92381f13-15eb-4ed4-ad57-19b0ef46b57d"), "Window", null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("e02814f3-4673-4cbb-b022-b3a7133b1a22"), "BulbTest", 24, null, true });
        }
    }
}
