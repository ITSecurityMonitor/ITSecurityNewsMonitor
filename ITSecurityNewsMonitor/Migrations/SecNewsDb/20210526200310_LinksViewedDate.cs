using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ITSecurityNewsMonitor.Migrations.SecNewsDb
{
    public partial class LinksViewedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinksViewed_NewsGroup_NewsGroupID",
                table: "LinksViewed");

            migrationBuilder.DropIndex(
                name: "IX_LinksViewed_NewsGroupID",
                table: "LinksViewed");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "LinksViewed");

            migrationBuilder.DropColumn(
                name: "NewsGroupID",
                table: "LinksViewed");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "LinksViewed",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "LinksViewed");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "LinksViewed",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewsGroupID",
                table: "LinksViewed",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinksViewed_NewsGroupID",
                table: "LinksViewed",
                column: "NewsGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_LinksViewed_NewsGroup_NewsGroupID",
                table: "LinksViewed",
                column: "NewsGroupID",
                principalTable: "NewsGroup",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
