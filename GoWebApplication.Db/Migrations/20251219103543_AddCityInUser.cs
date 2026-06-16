using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddCityInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idCity",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_users_idCity",
                table: "users",
                column: "idCity");

            migrationBuilder.AddForeignKey(
                name: "FK_users_cities_idCity",
                table: "users",
                column: "idCity",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_cities_idCity",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_idCity",
                table: "users");

            migrationBuilder.DropColumn(
                name: "idCity",
                table: "users");
        }
    }
}
