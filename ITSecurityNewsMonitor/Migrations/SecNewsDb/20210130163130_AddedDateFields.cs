using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class AddedDateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsLowLevelTags_NewsLowLevelTagID",
                table: "News");

            migrationBuilder.DropTable(
                name: "NewsLowLevelTags");

            migrationBuilder.DropIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests");

            migrationBuilder.DropIndex(
                name: "IX_News_NewsGroupId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_NewsLowLevelTagID",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsLowLevelTagID",
                table: "News");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "NewsGroup",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "NewsGroup",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "News",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "News",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests",
                column: "NewsGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsGroupId",
                table: "News",
                column: "NewsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LowLevelTagNews_NewsID",
                table: "LowLevelTagNews",
                column: "NewsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LowLevelTagNews");

            migrationBuilder.DropIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests");

            migrationBuilder.DropIndex(
                name: "IX_News_NewsGroupId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "NewsGroup");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "NewsGroup");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "News");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "NewsLowLevelTagID",
                table: "News",
                type: "integer",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_VoteRequests_NewsGroupId",
                table: "VoteRequests",
                column: "NewsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsGroupId",
                table: "News",
                column: "NewsGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsLowLevelTagID",
                table: "News",
                column: "NewsLowLevelTagID");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsLowLevelTags_NewsLowLevelTagID",
                table: "News",
                column: "NewsLowLevelTagID",
                principalTable: "NewsLowLevelTags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
