using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeyRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            //migrationBuilder.DropForeignKey(
            //    name: "FK_ratings_users_user_id", // Проверь имя в старых миграциях, обычно оно такое (FK_Таблица_ВнешняяТаблица_Колонка)
            //    table: "ratings");


            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_ratings",
            //    table: "ratings");


            migrationBuilder.DropIndex(
                name: "IX_ratings_user_id",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "id",
                table: "ratings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ratings",
                table: "ratings",
                columns: new[] { "user_id", "event_type_id" });


            migrationBuilder.AddForeignKey(
                name: "FK_ratings_users_user_id",
                table: "ratings",
                column: "user_id",
                principalTable: "users", 
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ratings",
                table: "ratings");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "ratings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ratings",
                table: "ratings",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_user_id",
                table: "ratings",
                column: "user_id");
        }
    }
}
