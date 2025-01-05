using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class makeonttoonerelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FireBaseTokens_UserId",
                table: "FireBaseTokens");

            migrationBuilder.CreateIndex(
                name: "IX_FireBaseTokens_UserId",
                table: "FireBaseTokens",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FireBaseTokens_UserId",
                table: "FireBaseTokens");

            migrationBuilder.CreateIndex(
                name: "IX_FireBaseTokens_UserId",
                table: "FireBaseTokens",
                column: "UserId");
        }
    }
}
