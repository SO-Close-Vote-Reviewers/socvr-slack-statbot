using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace SOCVR.Slack.StatBot.Migrations
{
    public partial class IncludeDisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Message_Room_RoomId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_Message_User_UserId", table: "Message");
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "User",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddForeignKey(
                name: "FK_Message_Room_RoomId",
                table: "Message",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_UserId",
                table: "Message",
                column: "UserId",
                principalTable: "User",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Message_Room_RoomId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_Message_User_UserId", table: "Message");
            migrationBuilder.DropColumn(name: "DisplayName", table: "User");
            migrationBuilder.AddForeignKey(
                name: "FK_Message_Room_RoomId",
                table: "Message",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_UserId",
                table: "Message",
                column: "UserId",
                principalTable: "User",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
