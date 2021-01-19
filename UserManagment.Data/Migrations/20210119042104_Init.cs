using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "management");

            migrationBuilder.CreateTable(
                name: "Schools",
                schema: "management",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Description = table.Column<string>(maxLength: 3000, nullable: true),
                    LogoId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
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
                    SchoolId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalSchema: "management",
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "management",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<byte>(nullable: false),
                    Sign = table.Column<string>(maxLength: 4, nullable: false),
                    FormTutorId = table.Column<Guid>(nullable: true),
                    SchoolId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Members_FormTutorId",
                        column: x => x.FormTutorId,
                        principalSchema: "management",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalSchema: "management",
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FormTutorId",
                schema: "management",
                table: "Groups",
                column: "FormTutorId",
                unique: true,
                filter: "[FormTutorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SchoolId",
                schema: "management",
                table: "Groups",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "Index_Code",
                schema: "management",
                table: "Groups",
                columns: new[] { "Id", "Number", "Sign" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                schema: "management",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_GroupId",
                schema: "management",
                table: "Members",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_SchoolId",
                schema: "management",
                table: "Members",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Groups_GroupId",
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
                name: "FK_Groups_Members_FormTutorId",
                schema: "management",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "management");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "management");

            migrationBuilder.DropTable(
                name: "Schools",
                schema: "management");
        }
    }
}
