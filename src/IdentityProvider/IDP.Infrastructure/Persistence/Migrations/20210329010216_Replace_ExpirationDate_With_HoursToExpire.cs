using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDP.Infrastructure.Persistence.Migrations
{
    public partial class Replace_ExpirationDate_With_HoursToExpire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityCodeExpirationDate",
                schema: "auth",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "SecurityCodeHoursToExpire",
                schema: "auth",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityCodeHoursToExpire",
                schema: "auth",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "SecurityCodeExpirationDate",
                schema: "auth",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
