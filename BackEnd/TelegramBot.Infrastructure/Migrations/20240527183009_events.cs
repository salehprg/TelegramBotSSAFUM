using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelegramBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class events : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostInfoEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PostInfoId = table.Column<int>(type: "integer", nullable: false),
                    TelegramId_PostId = table.Column<int>(type: "integer", nullable: false),
                    TelegramId_ChatId = table.Column<long>(type: "bigint", nullable: false),
                    PostStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInfoEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostInfoEvents_PostInfos_PostInfoId",
                        column: x => x.PostInfoId,
                        principalTable: "PostInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostInfoEvents_PostInfoId",
                table: "PostInfoEvents",
                column: "PostInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostInfoEvents");
        }
    }
}
