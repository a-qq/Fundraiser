using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Data.Migrations
{
    public partial class AddYearsOfEducationAndArchivizationProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "YearsOfEducation",
                schema: "management",
                table: "Schools",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                schema: "management",
                table: "Members",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                schema: "management",
                table: "Groups",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearsOfEducation",
                schema: "management",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                schema: "management",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                schema: "management",
                table: "Groups");
        }
    }
}
