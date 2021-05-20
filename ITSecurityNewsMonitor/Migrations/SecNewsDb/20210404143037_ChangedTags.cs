using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class ChangedTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HighLevelTagView");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "LowLevelTagNews");

            migrationBuilder.DropTable(
                name: "LowLevelTags");

            migrationBuilder.DropTable(
                name: "HighLevelTags");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    NewsID = table.Column<int>(type: "integer", nullable: true),
                    ViewID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tags_News_NewsID",
                        column: x => x.NewsID,
                        principalTable: "News",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tags_Views_ViewID",
                        column: x => x.ViewID,
                        principalTable: "Views",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NewsID",
                table: "Tags",
                column: "NewsID");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ViewID",
                table: "Tags",
                column: "ViewID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

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
                name: "LowLevelTags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HighLevelTagId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
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
                name: "Keywords",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LowLevelTagId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
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
                name: "LowLevelTagNews",
                columns: table => new
                {
                    LowLevelTagsID = table.Column<int>(type: "integer", nullable: false),
                    NewsID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LowLevelTagNews", x => new { x.LowLevelTagsID, x.NewsID });
                    table.ForeignKey(
                        name: "FK_LowLevelTagNews_LowLevelTags_LowLevelTagsID",
                        column: x => x.LowLevelTagsID,
                        principalTable: "LowLevelTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LowLevelTagNews_News_NewsID",
                        column: x => x.NewsID,
                        principalTable: "News",
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
                name: "IX_LowLevelTagNews_NewsID",
                table: "LowLevelTagNews",
                column: "NewsID");

            migrationBuilder.CreateIndex(
                name: "IX_LowLevelTags_HighLevelTagId",
                table: "LowLevelTags",
                column: "HighLevelTagId");
        }
    }
}
