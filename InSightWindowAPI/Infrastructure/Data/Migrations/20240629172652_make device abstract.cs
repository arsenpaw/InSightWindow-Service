using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class makedeviceabstract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("4baf082a-4749-4f6a-8c41-17a85f26377e"));

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("8172f5a4-656f-47cf-accf-6d97a9c8936c"));

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("7cc40a34-8d4b-459e-8c87-e02d397f9af3"), "BulbTest", 24, null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("dd08d963-3a5c-4d53-a941-f83e863d2f8e"), "Window", null, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("7cc40a34-8d4b-459e-8c87-e02d397f9af3"));

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("dd08d963-3a5c-4d53-a941-f83e863d2f8e"));

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("4baf082a-4749-4f6a-8c41-17a85f26377e"), "BulbTest", 24, null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("8172f5a4-656f-47cf-accf-6d97a9c8936c"), "Window", null, true });
        }
    }
}
