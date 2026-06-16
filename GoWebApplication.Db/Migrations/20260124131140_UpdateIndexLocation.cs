using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndexLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_address",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_locations_address_city_id",
                table: "locations",
                columns: new[] { "address", "city_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_address_city_id",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_locations_address",
                table: "locations",
                column: "address",
                unique: true);
        }
    }
}
