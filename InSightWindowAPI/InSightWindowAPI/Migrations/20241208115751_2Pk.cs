using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    /// <inheritdoc />
    public partial class _2Pk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFireBaseTokens",
                table: "UserFireBaseTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserFireBaseTokens_UserId",
                table: "UserFireBaseTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFireBaseTokens",
                table: "UserFireBaseTokens",
                columns: new[] { "UserId", "FireBaseTokenId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserFireBaseTokens_FireBaseTokenId",
                table: "UserFireBaseTokens",
                column: "FireBaseTokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFireBaseTokens",
                table: "UserFireBaseTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserFireBaseTokens_FireBaseTokenId",
                table: "UserFireBaseTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFireBaseTokens",
                table: "UserFireBaseTokens",
                columns: new[] { "FireBaseTokenId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserFireBaseTokens_UserId",
                table: "UserFireBaseTokens",
                column: "UserId");
        }
    }
}
