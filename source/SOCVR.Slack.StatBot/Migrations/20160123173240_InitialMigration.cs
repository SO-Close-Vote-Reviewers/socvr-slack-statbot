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
                    RoomId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                });
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ProfileId = table.Column<int>(nullable: false),
                    JoinedChatSystemAt = table.Column<DateTimeOffset>(nullable: false),
                    JoinedStackOverflowAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ProfileId);
                });
            migrationBuilder.CreateTable(
                name: "UserAlias",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    OriginalPosterId = table.Column<int>(nullable: false),
                    RevisionAuthorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlias", x => new { x.UserId, x.DisplayName });
                    table.UniqueConstraint("AK_UserAlias_OriginalPosterId", x => x.OriginalPosterId);
                    table.UniqueConstraint("AK_UserAlias_RevisionAuthorId", x => x.RevisionAuthorId);
                    table.ForeignKey(
                        name: "FK_UserAlias_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false),
                    CurrentText = table.Column<string>(nullable: true),
                    InitialRevisionTs = table.Column<DateTimeOffset>(nullable: false),
                    IsCloseVoteRequest = table.Column<bool>(nullable: false),
                    OneboxType = table.Column<int>(nullable: true),
                    OriginalPosterId = table.Column<int>(nullable: false),
                    PlainTextLinkCount = table.Column<int>(nullable: false),
                    RoomId = table.Column<int>(nullable: false),
                    StarCount = table.Column<int>(nullable: false),
                    Tags = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_UserAlias_OriginalPosterId",
                        column: x => x.OriginalPosterId,
                        principalTable: "UserAlias",
                        principalColumn: "OriginalPosterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "MessageRevision",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false),
                    RevisionNumber = table.Column<int>(nullable: false),
                    RevisionAuthorId = table.Column<int>(nullable: false),
                    RevisionMadeAt = table.Column<DateTimeOffset>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRevision", x => new { x.MessageId, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_MessageRevision_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageRevision_UserAlias_RevisionAuthorId",
                        column: x => x.RevisionAuthorId,
                        principalTable: "UserAlias",
                        principalColumn: "RevisionAuthorId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("MessageRevision");
            migrationBuilder.DropTable("Message");
            migrationBuilder.DropTable("UserAlias");
            migrationBuilder.DropTable("Room");
            migrationBuilder.DropTable("User");
        }
    }
}
