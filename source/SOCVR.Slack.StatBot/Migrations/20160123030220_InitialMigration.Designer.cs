using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SOCVR.Slack.StatBot.Database;

namespace SOCVR.Slack.StatBot.Migrations
{
    [DbContext(typeof(MessageStorage))]
    [Migration("20160123030220_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Message", b =>
                {
                    b.Property<int>("Id");

                    b.Property<bool>("HasLinks");

                    b.Property<bool>("HasTagFormatting");

                    b.Property<bool>("IsCloseVoteRequest");

                    b.Property<int?>("OneboxType");

                    b.Property<DateTimeOffset>("PostedAt");

                    b.Property<int>("RoomId");

                    b.Property<int>("StarCount");

                    b.Property<int>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Room", b =>
                {
                    b.Property<int>("RoomId");

                    b.HasKey("RoomId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.User", b =>
                {
                    b.Property<int>("ProfileId");

                    b.HasKey("ProfileId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Message", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.Room")
                        .WithMany()
                        .HasForeignKey("RoomId");

                    b.HasOne("SOCVR.Slack.StatBot.Database.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
