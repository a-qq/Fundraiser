using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Data.Migrations
{
    public partial class RemoveCodeIndexUniquness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Index_Code",
                schema: "management",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "Index_Code",
                schema: "management",
                table: "Groups",
                columns: new[] { "Number", "Sign" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Index_Code",
                schema: "management",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "Index_Code",
                schema: "management",
                table: "Groups",
                columns: new[] { "Id", "Number", "Sign" },
                unique: true);
        }
    }
}
