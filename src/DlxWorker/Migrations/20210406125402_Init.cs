using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DlxWorker.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dlx");

            migrationBuilder.CreateTable(
                name: "DeadLetterEvents",
                schema: "dlx",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    HandledAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeadLetterEvents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeadLetterEvents",
                schema: "dlx");
        }
    }
}
