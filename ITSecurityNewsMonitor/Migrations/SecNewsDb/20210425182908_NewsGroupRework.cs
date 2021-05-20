using Microsoft.EntityFrameworkCore.Migrations;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class NewsGroupRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsGroup_NewsGroupId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_NewsGroupId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsGroupId",
                table: "News");

            migrationBuilder.CreateTable(
                name: "NewsNewsGroup",
                columns: table => new
                {
                    NewsGroupsID = table.Column<int>(type: "integer", nullable: false),
                    NewsID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsNewsGroup", x => new { x.NewsGroupsID, x.NewsID });
                    table.ForeignKey(
                        name: "FK_NewsNewsGroup_News_NewsID",
                        column: x => x.NewsID,
                        principalTable: "News",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsNewsGroup_NewsGroup_NewsGroupsID",
                        column: x => x.NewsGroupsID,
                        principalTable: "NewsGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsNewsGroup_NewsID",
                table: "NewsNewsGroup",
                column: "NewsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsNewsGroup");

            migrationBuilder.AddColumn<int>(
                name: "NewsGroupId",
                table: "News",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsGroupId",
                table: "News",
                column: "NewsGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsGroup_NewsGroupId",
                table: "News",
                column: "NewsGroupId",
                principalTable: "NewsGroup",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
