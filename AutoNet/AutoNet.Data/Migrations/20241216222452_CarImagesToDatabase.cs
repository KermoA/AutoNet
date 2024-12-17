using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class CarImagesToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileToDatabases_AspNetUsers_UserId",
                table: "FileToDatabases");

            migrationBuilder.DropIndex(
                name: "IX_FileToDatabases_UserId",
                table: "FileToDatabases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FileToDatabases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FileToDatabases",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FileToDatabases_UserId",
                table: "FileToDatabases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileToDatabases_AspNetUsers_UserId",
                table: "FileToDatabases",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
