using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace SOCVR.Slack.StatBot.Migrations
{
    public partial class AddParsedTranscriptPageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Message_UserAlias_OriginalPosterId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_Message_Room_RoomId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_MessageRevision_Message_MessageId", table: "MessageRevision");
            migrationBuilder.DropForeignKey(name: "FK_MessageRevision_UserAlias_RevisionAuthorId", table: "MessageRevision");
            migrationBuilder.DropForeignKey(name: "FK_UserAlias_User_UserId", table: "UserAlias");
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
            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserAlias_OriginalPosterId",
                table: "Message",
                column: "OriginalPosterId",
                principalTable: "UserAlias",
                principalColumn: "OriginalPosterId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Message_Room_RoomId",
                table: "Message",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_MessageRevision_Message_MessageId",
                table: "MessageRevision",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_MessageRevision_UserAlias_RevisionAuthorId",
                table: "MessageRevision",
                column: "RevisionAuthorId",
                principalTable: "UserAlias",
                principalColumn: "RevisionAuthorId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_UserAlias_User_UserId",
                table: "UserAlias",
                column: "UserId",
                principalTable: "User",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Message_UserAlias_OriginalPosterId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_Message_Room_RoomId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_MessageRevision_Message_MessageId", table: "MessageRevision");
            migrationBuilder.DropForeignKey(name: "FK_MessageRevision_UserAlias_RevisionAuthorId", table: "MessageRevision");
            migrationBuilder.DropForeignKey(name: "FK_UserAlias_User_UserId", table: "UserAlias");
            migrationBuilder.DropTable("ParsedTranscriptPage");
            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserAlias_OriginalPosterId",
                table: "Message",
                column: "OriginalPosterId",
                principalTable: "UserAlias",
                principalColumn: "OriginalPosterId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Message_Room_RoomId",
                table: "Message",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_MessageRevision_Message_MessageId",
                table: "MessageRevision",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_MessageRevision_UserAlias_RevisionAuthorId",
                table: "MessageRevision",
                column: "RevisionAuthorId",
                principalTable: "UserAlias",
                principalColumn: "RevisionAuthorId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_UserAlias_User_UserId",
                table: "UserAlias",
                column: "UserId",
                principalTable: "User",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
