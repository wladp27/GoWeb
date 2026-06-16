using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_cities_idCity",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "idCity",
                table: "users",
                newName: "id_city");

            migrationBuilder.RenameIndex(
                name: "IX_users_idCity",
                table: "users",
                newName: "IX_users_id_city");

            migrationBuilder.AddForeignKey(
                name: "FK_users_cities_id_city",
                table: "users",
                column: "id_city",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_cities_id_city",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "id_city",
                table: "users",
                newName: "idCity");

            migrationBuilder.RenameIndex(
                name: "IX_users_id_city",
                table: "users",
                newName: "IX_users_idCity");

            migrationBuilder.AddForeignKey(
                name: "FK_users_cities_idCity",
                table: "users",
                column: "idCity",
                principalTable: "cities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
