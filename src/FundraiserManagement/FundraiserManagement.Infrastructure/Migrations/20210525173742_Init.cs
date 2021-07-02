using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FundraiserManagement.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fund");

            migrationBuilder.EnsureSchema(
                name: "idreq");

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "fund",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true),
                    SchoolId = table.Column<Guid>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    IsTreasurer = table.Column<bool>(nullable: false),
                    IsFormTutor = table.Column<bool>(nullable: false),
                    Card_Number = table.Column<string>(nullable: true),
                    Card_Month = table.Column<byte>(nullable: true),
                    Card_Year = table.Column<byte>(nullable: true),
                    Card_CVC = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Fundraisers",
                schema: "fund",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Goal = table.Column<decimal>(nullable: true),
                    IsGoalShared = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    GroupId = table.Column<Guid>(nullable: true),
                    SchoolId = table.Column<Guid>(nullable: false),
                    ManagerId = table.Column<Guid>(nullable: true),
                    Description = table.Column<string>(maxLength: 3000, nullable: true),
                    Range = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fundraisers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fundraisers_Members_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "fund",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Participations",
                schema: "fund",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FundraisingId = table.Column<Guid>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participations_Fundraisers_FundraisingId",
                        column: x => x.FundraisingId,
                        principalSchema: "fund",
                        principalTable: "Fundraisers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participations_Members_ParticipantId",
                        column: x => x.ParticipantId,
                        principalSchema: "fund",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "fund",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AddedAt = table.Column<DateTimeOffset>(nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    InCash = table.Column<bool>(nullable: false),
                    ParticipationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Participations_ParticipationId",
                        column: x => x.ParticipationId,
                        principalSchema: "fund",
                        principalTable: "Participations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fundraisers_ManagerId",
                schema: "fund",
                table: "Fundraisers",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_FundraisingId",
                schema: "fund",
                table: "Participations",
                column: "FundraisingId");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_ParticipantId",
                schema: "fund",
                table: "Participations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ParticipationId",
                schema: "fund",
                table: "Payments",
                column: "ParticipationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments",
                schema: "fund");

            migrationBuilder.DropTable(
                name: "requests",
                schema: "idreq");

            migrationBuilder.DropTable(
                name: "Participations",
                schema: "fund");

            migrationBuilder.DropTable(
                name: "Fundraisers",
                schema: "fund");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "fund");
        }
    }
}
