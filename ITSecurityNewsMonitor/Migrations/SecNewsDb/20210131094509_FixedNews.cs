using Microsoft.EntityFrameworkCore.Migrations;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class FixedNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsGroup_LowLevelTags_LowLevelTagId",
                table: "NewsGroup");

            migrationBuilder.DropIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests");

            migrationBuilder.DropIndex(
                name: "IX_NewsGroup_LowLevelTagId",
                table: "NewsGroup");

            migrationBuilder.DropColumn(
                name: "LowLevelTagId",
                table: "NewsGroup");

            migrationBuilder.AddColumn<string>(
                name: "Homepage",
                table: "Sources",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests",
                column: "NewsGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests");

            migrationBuilder.DropColumn(
                name: "Homepage",
                table: "Sources");

            migrationBuilder.AddColumn<int>(
                name: "LowLevelTagId",
                table: "NewsGroup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests",
                column: "NewsGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsGroup_LowLevelTagId",
                table: "NewsGroup",
                column: "LowLevelTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsGroup_LowLevelTags_LowLevelTagId",
                table: "NewsGroup",
                column: "LowLevelTagId",
                principalTable: "LowLevelTags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
