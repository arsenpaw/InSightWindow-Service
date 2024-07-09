using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addfieldstowindow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("7cc40a34-8d4b-459e-8c87-e02d397f9af3"));

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: new Guid("dd08d963-3a5c-4d53-a941-f83e863d2f8e"));

            migrationBuilder.RenameColumn(
                name: "isOpen",
                table: "Devices",
                newName: "IsOpenButton");

            migrationBuilder.AlterColumn<string>(
                name: "IsOpenButton",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsProtected",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Temparature",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isAlarm",
                table: "Devices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isRain",
                table: "Devices",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IsProtected",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Temparature",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isAlarm",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isRain",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "IsOpenButton",
                table: "Devices",
                newName: "isOpen");

            migrationBuilder.AlterColumn<bool>(
                name: "isOpen",
                table: "Devices",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "LightPowered", "UserId", "isOn" },
                values: new object[] { new Guid("7cc40a34-8d4b-459e-8c87-e02d397f9af3"), "BulbTest", 24, null, true });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "DeviceType", "UserId", "isOpen" },
                values: new object[] { new Guid("dd08d963-3a5c-4d53-a941-f83e863d2f8e"), "Window", null, true });
        }
    }
}
