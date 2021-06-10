using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class LinksViewed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoteRequests");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.CreateTable(
                name: "LinksViewed",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerID = table.Column<string>(type: "text", nullable: true),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    NewsId = table.Column<int>(type: "integer", nullable: false),
                    NewsGroupID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinksViewed", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LinksViewed_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinksViewed_NewsGroup_NewsGroupID",
                        column: x => x.NewsGroupID,
                        principalTable: "NewsGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinksViewed_NewsGroupID",
                table: "LinksViewed",
                column: "NewsGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LinksViewed_NewsId",
                table: "LinksViewed",
                column: "NewsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinksViewed");

            migrationBuilder.CreateTable(
                name: "VoteRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    NewsGroupId = table.Column<int>(type: "integer", nullable: false),
                    OwnerID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VoteRequests_NewsGroup_NewsGroupId",
                        column: x => x.NewsGroupId,
                        principalTable: "NewsGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Criticality = table.Column<bool>(type: "boolean", nullable: false),
                    NewsGroupId = table.Column<int>(type: "integer", nullable: false),
                    OwnerID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Votes_NewsGroup_NewsGroupId",
                        column: x => x.NewsGroupId,
                        principalTable: "NewsGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests",
                column: "NewsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_NewsGroupId",
                table: "Votes",
                column: "NewsGroupId");
        }
    }
}
