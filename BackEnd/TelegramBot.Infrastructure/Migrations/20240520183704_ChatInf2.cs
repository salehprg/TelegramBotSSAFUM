using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelegramBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatInf2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostInfos_ChannelInfoEntity_channelInfoId",
                table: "PostInfos");

            migrationBuilder.DropTable(
                name: "ChannelInfoEntity");

            migrationBuilder.DropIndex(
                name: "IX_PostInfos_channelInfoId",
                table: "PostInfos");

            migrationBuilder.DropColumn(
                name: "channelInfoId",
                table: "PostInfos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "channelInfoId",
                table: "PostInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChannelInfoEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChannelTelegramId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelInfoEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostInfos_channelInfoId",
                table: "PostInfos",
                column: "channelInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostInfos_ChannelInfoEntity_channelInfoId",
                table: "PostInfos",
                column: "channelInfoId",
                principalTable: "ChannelInfoEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
