using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventAndLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     



            

           


            migrationBuilder.Sql(@"
                                    INSERT INTO locations (address, location_latitude, location_longitude, city_id)
                                    SELECT address, MAX(location_latitude), MAX(location_longitude), MAX(city_id)
                                    FROM events 
                                    WHERE address IS NOT NULL
                                    GROUP BY address;
                ");

            migrationBuilder.Sql(@"
                                    UPDATE events e
                                    JOIN locations l ON e.address = l.address AND e.city_id = l.city_id
                                    SET e.location_id = l.id;
                                        ");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_locations_location_id",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_locations_cities_city_id",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_locations_city_id",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_Events_location_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "city_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "Events");
        }
    }
}
