using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Data.Migrations
{
    public partial class TrackAsAddedWhenKeyValueIsSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupID",
                schema: "management",
                table: "Groups",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "management",
                table: "Groups",
                newName: "GroupID");
        }
    }
}
