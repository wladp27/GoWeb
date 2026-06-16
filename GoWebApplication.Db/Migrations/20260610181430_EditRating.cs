using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class EditRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratings_users_user_id",
                table: "ratings");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ratings",
                newName: "user_name");

            migrationBuilder.AddForeignKey(
                name: "FK_ratings_users_user_name",
                table: "ratings",
                column: "user_name",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratings_users_user_name",
                table: "ratings");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "ratings",
                newName: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ratings_users_user_id",
                table: "ratings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
