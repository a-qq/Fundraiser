using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Data.Migrations
{
    public partial class AddUserIsActiveProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SchoolID",
                schema: "management",
                table: "Schools",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "management",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "management",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "management",
                table: "Schools",
                newName: "SchoolID");
        }
    }
}
