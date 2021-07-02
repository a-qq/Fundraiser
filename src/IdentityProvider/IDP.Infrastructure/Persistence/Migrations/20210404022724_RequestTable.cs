using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDP.Infrastructure.Persistence.Migrations
{
    public partial class RequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "idreq");

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "idreq",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requests",
                schema: "idreq");
        }
    }
}
