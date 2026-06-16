using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_cities_city_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "address",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "location_latitude",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "location_longitude",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "city_id",
                table: "Events",
                newName: "CityId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_city_id",
                table: "Events",
                newName: "IX_Events_CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_cities_CityId",
                table: "Events",
                column: "CityId",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_cities_CityId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Events",
                newName: "city_id");

            migrationBuilder.RenameIndex(
                name: "IX_Events_CityId",
                table: "Events",
                newName: "IX_Events_city_id");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Events",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "location_latitude",
                table: "Events",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "location_longitude",
                table: "Events",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_cities_city_id",
                table: "Events",
                column: "city_id",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
