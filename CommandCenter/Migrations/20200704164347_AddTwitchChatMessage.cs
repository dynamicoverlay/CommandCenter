using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommandCenter.Migrations
{
    public partial class AddTwitchChatMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwitchChatMessages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ReceivedTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    RoomId = table.Column<string>(nullable: true),
                    Channel = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchChatMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwitchChatMessages_UserId",
                table: "TwitchChatMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchChatMessages");
        }
    }
}
