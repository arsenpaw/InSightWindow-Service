using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class removeuselesscode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("4baf082a-4749-4f6a-8c41-17a85f26377e"), "BulbTest", 24, null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("8172f5a4-656f-47cf-accf-6d97a9c8936c"), "Window", null, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("56af4916-703d-40fc-9268-093e6db2d4df"), "Window", null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("7184508c-d179-4e40-acac-e701502f0af8"), "BulbTest", 24, null, true });
        }
    }
}
