using Microsoft.EntityFrameworkCore.Migrations;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class Track_Story_Assignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AssignedToStory",
                table: "News",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToStory",
                table: "News");
        }
    }
}
