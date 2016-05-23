using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(nullable: false),
                    Topic = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Word",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "WordOccurence",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArticleId = table.Column<int>(nullable: false),
                    TimesOccured = table.Column<int>(nullable: false),
                    WordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordOccurence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordOccurence_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordOccurence_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("WordOccurence");
            migrationBuilder.DropTable("Article");
            migrationBuilder.DropTable("Word");
        }
    }
}
