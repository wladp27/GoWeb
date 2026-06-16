using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWebApplication.Db.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEventNullLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

               migrationBuilder.Sql(@"
                                    UPDATE events
                                    SET location_id = 4
                                    WHERE location_id is null;
                                    
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
