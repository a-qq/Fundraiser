using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Data.Migrations
{
    public partial class TreasurerNavigationPropertyAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TreasurerId",
                schema: "management",
                table: "Groups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TreasurerId",
                schema: "management",
                table: "Groups",
                column: "TreasurerId",
                unique: true,
                filter: "[TreasurerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Members_TreasurerId",
                schema: "management",
                table: "Groups",
                column: "TreasurerId",
                principalSchema: "management",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Members_TreasurerId",
                schema: "management",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TreasurerId",
                schema: "management",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TreasurerId",
                schema: "management",
                table: "Groups");
        }
    }
}
