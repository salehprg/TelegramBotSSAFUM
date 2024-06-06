using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class eventsreply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Repliyed_TelegramId_ChatId",
                table: "PostInfoEvents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Repliyed_TelegramId_PostId",
                table: "PostInfoEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Repliyed_TelegramId_ChatId",
                table: "PostInfoEvents");

            migrationBuilder.DropColumn(
                name: "Repliyed_TelegramId_PostId",
                table: "PostInfoEvents");
        }
    }
}
