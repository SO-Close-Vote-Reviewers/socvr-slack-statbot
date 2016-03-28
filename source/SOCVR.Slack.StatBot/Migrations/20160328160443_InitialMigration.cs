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
                name: "ParsedTranscriptPage",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedTranscriptPage", x => new { x.RoomId, x.Date });
                    table.ForeignKey(
                        name: "FK_ParsedTranscriptPage_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "UserAlias",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlias", x => new { x.UserId, x.DisplayName });
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
                    MessageId = table.Column<long>(nullable: false),
                    AuthorDisplayName = table.Column<string>(nullable: true),
                    AuthorProfileId = table.Column<int>(nullable: false),
                    CurrentMarkdownContent = table.Column<string>(nullable: true),
                    CurrentText = table.Column<string>(nullable: true),
                    InitialRevisionTs = table.Column<DateTimeOffset>(nullable: false),
                    IsCloseVoteRequest = table.Column<bool>(nullable: false),
                    OneboxType = table.Column<int>(nullable: true),
                    PlainTextLinkCount = table.Column<int>(nullable: false),
                    RoomId = table.Column<int>(nullable: false),
                    StarCount = table.Column<int>(nullable: false),
                    Tags = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_UserAlias_AuthorProfileId_AuthorDisplayName",
                        columns: x => new { x.AuthorProfileId, x.AuthorDisplayName },
                        principalTable: "UserAlias",
                        principalColumns: new[] { "UserId", "DisplayName" },
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "MessageRevision",
                columns: table => new
                {
                    MessageId = table.Column<long>(nullable: false),
                    RevisionNumber = table.Column<int>(nullable: false),
                    RevisionAuthorDisplayName = table.Column<string>(nullable: true),
                    RevisionAuthorId = table.Column<int>(nullable: false),
                    RevisionAuthorUserId = table.Column<int>(nullable: true),
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
                        name: "FK_MessageRevision_UserAlias_RevisionAuthorUserId_RevisionAuthorDisplayName",
                        columns: x => new { x.RevisionAuthorUserId, x.RevisionAuthorDisplayName },
                        principalTable: "UserAlias",
                        principalColumns: new[] { "UserId", "DisplayName" },
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("MessageRevision");
            migrationBuilder.DropTable("ParsedTranscriptPage");
            migrationBuilder.DropTable("Message");
            migrationBuilder.DropTable("Room");
            migrationBuilder.DropTable("UserAlias");
            migrationBuilder.DropTable("User");
        }
    }
}
