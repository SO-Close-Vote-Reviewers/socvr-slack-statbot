using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace SOCVR.Slack.StatBot.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                });
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ProfileId);
                });
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    HasLinks = table.Column<bool>(nullable: false),
                    HasTagFormatting = table.Column<bool>(nullable: false),
                    IsCloseVoteRequest = table.Column<bool>(nullable: false),
                    OneboxType = table.Column<int>(nullable: true),
                    PostedAt = table.Column<DateTimeOffset>(nullable: false),
                    RoomId = table.Column<int>(nullable: false),
                    StarCount = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Message");
            migrationBuilder.DropTable("Room");
            migrationBuilder.DropTable("User");
        }
    }
}
