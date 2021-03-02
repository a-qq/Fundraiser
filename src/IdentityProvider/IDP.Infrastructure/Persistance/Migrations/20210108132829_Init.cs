﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDP.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(nullable: false),
                    HashedPassword = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    SecurityCode = table.Column<string>(nullable: true),
                    SecurityCodeIssuedAt = table.Column<DateTime>(nullable: true),
                    SecurityCodeExpirationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Subject", x => x.Subject);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                schema: "auth",
                columns: table => new
                {
                    ClaimId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    UserSubject = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_Claims_Users_UserSubject",
                        column: x => x.UserSubject,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Subject",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_UserSubject",
                schema: "auth",
                table: "Claims",
                column: "UserSubject");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "auth",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "auth");
        }
    }
}
