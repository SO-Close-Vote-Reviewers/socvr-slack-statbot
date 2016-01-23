using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SOCVR.Slack.StatBot.Database;

namespace SOCVR.Slack.StatBot.Migrations
{
    [DbContext(typeof(MessageStorage))]
    [Migration("20160123173240_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Message", b =>
                {
                    b.Property<int>("MessageId");

                    b.Property<string>("CurrentText");

                    b.Property<DateTimeOffset>("InitialRevisionTs");

                    b.Property<bool>("IsCloseVoteRequest");

                    b.Property<int?>("OneboxType");

                    b.Property<int>("OriginalPosterId");

                    b.Property<int>("PlainTextLinkCount");

                    b.Property<int>("RoomId");

                    b.Property<int>("StarCount");

                    b.Property<int>("Tags");

                    b.HasKey("MessageId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.MessageRevision", b =>
                {
                    b.Property<int>("MessageId");

                    b.Property<int>("RevisionNumber");

                    b.Property<int>("RevisionAuthorId");

                    b.Property<DateTimeOffset>("RevisionMadeAt");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("MessageId", "RevisionNumber");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Room", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("RoomId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.User", b =>
                {
                    b.Property<int>("ProfileId");

                    b.Property<DateTimeOffset>("JoinedChatSystemAt");

                    b.Property<DateTimeOffset>("JoinedStackOverflowAt");

                    b.HasKey("ProfileId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.UserAlias", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("DisplayName");

                    b.Property<int>("OriginalPosterId");

                    b.Property<int>("RevisionAuthorId");

                    b.HasKey("UserId", "DisplayName");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Message", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.UserAlias")
                        .WithMany()
                        .HasForeignKey("OriginalPosterId")
                        .HasPrincipalKey("OriginalPosterId");

                    b.HasOne("SOCVR.Slack.StatBot.Database.Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.MessageRevision", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.Message")
                        .WithMany()
                        .HasForeignKey("MessageId");

                    b.HasOne("SOCVR.Slack.StatBot.Database.UserAlias")
                        .WithMany()
                        .HasForeignKey("RevisionAuthorId")
                        .HasPrincipalKey("RevisionAuthorId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.UserAlias", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
