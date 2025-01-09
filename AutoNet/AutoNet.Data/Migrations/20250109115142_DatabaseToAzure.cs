using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseToAzure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FileToDatabases_CarId",
                table: "FileToDatabases",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileToDatabases_Cars_CarId",
                table: "FileToDatabases",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileToDatabases_Cars_CarId",
                table: "FileToDatabases");

            migrationBuilder.DropIndex(
                name: "IX_FileToDatabases_CarId",
                table: "FileToDatabases");
        }
    }
}
