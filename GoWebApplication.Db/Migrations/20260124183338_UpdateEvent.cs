using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_cities_CityId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_cities_CityId",
                table: "Events",
                column: "CityId",
                principalTable: "cities",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_cities_CityId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_cities_CityId",
                table: "Events",
                column: "CityId",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
