using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class MigratedRailsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HighLevelTags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HighLevelTags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NewsLowLevelTags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLowLevelTags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    OwnerID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LowLevelTags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    HighLevelTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LowLevelTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LowLevelTags_HighLevelTags_HighLevelTagId",
                        column: x => x.HighLevelTagId,
                        principalTable: "HighLevelTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HighLevelTagView",
                columns: table => new
                {
                    HighLevelTagsID = table.Column<int>(type: "integer", nullable: false),
                    ViewsID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HighLevelTagView", x => new { x.HighLevelTagsID, x.ViewsID });
                    table.ForeignKey(
                        name: "FK_HighLevelTagView_HighLevelTags_HighLevelTagsID",
                        column: x => x.HighLevelTagsID,
                        principalTable: "HighLevelTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HighLevelTagView_Views_ViewsID",
                        column: x => x.ViewsID,
                        principalTable: "Views",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    LowLevelTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Keywords_LowLevelTags_LowLevelTagId",
                        column: x => x.LowLevelTagId,
                        principalTable: "LowLevelTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsGroup",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    LowLevelTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsGroup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NewsGroup_LowLevelTags_LowLevelTagId",
                        column: x => x.LowLevelTagId,
                        principalTable: "LowLevelTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Headline = table.Column<string>(type: "text", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true),
                    ManuallyAssigned = table.Column<bool>(type: "boolean", nullable: false),
                    NewsGroupId = table.Column<int>(type: "integer", nullable: false),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    NewsLowLevelTagID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.ID);
                    table.ForeignKey(
                        name: "FK_News_NewsGroup_NewsGroupId",
                        column: x => x.NewsGroupId,
                        principalTable: "NewsGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_News_NewsLowLevelTags_NewsLowLevelTagID",
                        column: x => x.NewsLowLevelTagID,
                        principalTable: "NewsLowLevelTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_News_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerID = table.Column<string>(type: "text", nullable: true),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    NewsGroupId = table.Column<int>(type: "integer", nullable: false)
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
                    OwnerID = table.Column<string>(type: "text", nullable: true),
                    Criticality = table.Column<bool>(type: "boolean", nullable: false),
                    NewsGroupId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_HighLevelTagView_ViewsID",
                table: "HighLevelTagView",
                column: "ViewsID");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_LowLevelTagId",
                table: "Keywords",
                column: "LowLevelTagId");

            migrationBuilder.CreateIndex(
                name: "IX_LowLevelTags_HighLevelTagId",
                table: "LowLevelTags",
                column: "HighLevelTagId");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsGroupId",
                table: "News",
                column: "NewsGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsLowLevelTagID",
                table: "News",
                column: "NewsLowLevelTagID");

            migrationBuilder.CreateIndex(
                name: "IX_News_SourceId",
                table: "News",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsGroup_LowLevelTagId",
                table: "NewsGroup",
                column: "LowLevelTagId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests",
                column: "NewsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_NewsGroupId",
                table: "Votes",
                column: "NewsGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HighLevelTagView");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "VoteRequests");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Views");

            migrationBuilder.DropTable(
                name: "NewsLowLevelTags");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "NewsGroup");

            migrationBuilder.DropTable(
                name: "LowLevelTags");

            migrationBuilder.DropTable(
                name: "HighLevelTags");
        }
    }
}
