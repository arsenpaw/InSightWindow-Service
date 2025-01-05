using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class BugFixesamdTemprempvesomeproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Users_OwnerId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_OwnerId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Devices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Devices_OwnerId",
                table: "Devices",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Users_OwnerId",
                table: "Devices",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
