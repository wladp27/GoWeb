using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCityLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_locations_cities_city_id",
                table: "locations");

            migrationBuilder.AddForeignKey(
                name: "FK_locations_cities_city_id",
                table: "locations",
                column: "city_id",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_locations_cities_city_id",
                table: "locations");

            migrationBuilder.AddForeignKey(
                name: "FK_locations_cities_city_id",
                table: "locations",
                column: "city_id",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
