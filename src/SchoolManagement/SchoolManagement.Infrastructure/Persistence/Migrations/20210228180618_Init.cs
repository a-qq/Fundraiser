using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Infrastructure.Persistence.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "management");

            migrationBuilder.CreateTable(
                "Schools",
                schema: "management",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Description = table.Column<string>(maxLength: 3000, nullable: true),
                    GroupMembersLimit = table.Column<int>(nullable: true),
                    YearsOfEducation = table.Column<byte>(nullable: false),
                    LogoId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Schools", x => x.Id); });

            migrationBuilder.CreateTable(
                "Members",
                schema: "management",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 200, nullable: false),
                    LastName = table.Column<string>(maxLength: 200, nullable: false),
                    Role = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    SchoolId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        "FK_Members_Schools_SchoolId",
                        x => x.SchoolId,
                        principalSchema: "management",
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Groups",
                schema: "management",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Number = table.Column<byte>(nullable: false),
                    Sign = table.Column<string>(maxLength: 4, nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    FormTutorId = table.Column<Guid>(nullable: true),
                    TreasurerId = table.Column<Guid>(nullable: true),
                    SchoolId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        "FK_Groups_Members_FormTutorId",
                        x => x.FormTutorId,
                        principalSchema: "management",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Groups_Schools_SchoolId",
                        x => x.SchoolId,
                        principalSchema: "management",
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Groups_Members_TreasurerId",
                        x => x.TreasurerId,
                        principalSchema: "management",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Groups_FormTutorId",
                schema: "management",
                table: "Groups",
                column: "FormTutorId");

            migrationBuilder.CreateIndex(
                "IX_Groups_SchoolId",
                schema: "management",
                table: "Groups",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                "IX_Groups_TreasurerId",
                schema: "management",
                table: "Groups",
                column: "TreasurerId",
                unique: true,
                filter: "[TreasurerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "Index_Code",
                schema: "management",
                table: "Groups",
                columns: new[] {"Number", "Sign"});

            migrationBuilder.CreateIndex(
                "IX_Members_Email",
                schema: "management",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Members_GroupId",
                schema: "management",
                table: "Members",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                "IX_Members_SchoolId",
                schema: "management",
                table: "Members",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                "FK_Members_Groups_GroupId",
                schema: "management",
                table: "Members",
                column: "GroupId",
                principalSchema: "management",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Groups_Members_FormTutorId",
                schema: "management",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                "FK_Groups_Members_TreasurerId",
                schema: "management",
                table: "Groups");

            migrationBuilder.DropTable(
                "Members",
                "management");

            migrationBuilder.DropTable(
                "Groups",
                "management");

            migrationBuilder.DropTable(
                "Schools",
                "management");
        }
    }
}