using Microsoft.EntityFrameworkCore.Migrations;

namespace DlxWorker.Migrations
{
    public partial class AddReasonProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "dlx",
                table: "DeadLetterEvents",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "dlx",
                table: "DeadLetterEvents");
        }
    }
}
