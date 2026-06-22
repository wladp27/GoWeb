using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratings_users_user_name",
                table: "ratings");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "ratings",
                newName: "user_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rating_Value_Range",
                table: "ratings",
                sql: "`value` >= 0 AND `value` <= 100");

            migrationBuilder.AddForeignKey(
                name: "FK_ratings_users_user_id",
                table: "ratings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratings_users_user_id",
                table: "ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Rating_Value_Range",
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
    }
}
