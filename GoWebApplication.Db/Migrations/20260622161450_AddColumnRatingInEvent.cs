using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnRatingInEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Rating_Value_Range",
                table: "ratings");

            migrationBuilder.AddColumn<int>(
                name: "required_rating",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rating_Value_Range1",
                table: "ratings",
                sql: "`value` >= 0 AND `value` <= 100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rating_Value_Range",
                table: "Events",
                sql: "`required_rating` >= 0 AND `required_rating` <= 100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Rating_Value_Range1",
                table: "ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Rating_Value_Range",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "required_rating",
                table: "Events");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rating_Value_Range",
                table: "ratings",
                sql: "`value` >= 0 AND `value` <= 100");
        }
    }
}
