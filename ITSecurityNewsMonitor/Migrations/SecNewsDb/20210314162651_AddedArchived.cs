using Microsoft.EntityFrameworkCore.Migrations;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class AddedArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "NewsGroup",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "NewsGroup");
        }
    }
}
