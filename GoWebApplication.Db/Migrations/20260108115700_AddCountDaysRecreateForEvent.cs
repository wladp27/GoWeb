using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddCountDaysRecreateForEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "count_days_recreate",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "limit_count_days",
                table: "Events",
                sql: "count_days_recreate > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "limit_count_days",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "count_days_recreate",
                table: "Events");
        }
    }
}
