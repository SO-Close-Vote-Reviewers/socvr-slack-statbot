using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SOCVR.Slack.StatBot.Database;

namespace SOCVR.Slack.StatBot.Migrations
{
    [DbContext(typeof(MessageStorage))]
    [Migration("20160328160443_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Message", b =>
                {
                    b.Property<long>("MessageId");

                    b.Property<string>("AuthorDisplayName");

                    b.Property<int>("AuthorProfileId");

                    b.Property<string>("CurrentMarkdownContent");

                    b.Property<string>("CurrentText");

                    b.Property<DateTimeOffset>("InitialRevisionTs");

                    b.Property<bool>("IsCloseVoteRequest");

                    b.Property<int?>("OneboxType");

                    b.Property<int>("PlainTextLinkCount");

                    b.Property<int>("RoomId");

                    b.Property<int>("StarCount");

                    b.Property<int>("Tags");

                    b.HasKey("MessageId");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.MessageRevision", b =>
                {
                    b.Property<long>("MessageId");

                    b.Property<int>("RevisionNumber");

                    b.Property<string>("RevisionAuthorDisplayName");

                    b.Property<int>("RevisionAuthorId");

                    b.Property<int?>("RevisionAuthorUserId");

                    b.Property<DateTimeOffset>("RevisionMadeAt");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("MessageId", "RevisionNumber");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.ParsedTranscriptPage", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<DateTime>("Date");

                    b.HasKey("RoomId", "Date");
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

                    b.HasKey("UserId", "DisplayName");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.Message", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.Room")
                        .WithMany()
                        .HasForeignKey("RoomId");

                    b.HasOne("SOCVR.Slack.StatBot.Database.UserAlias")
                        .WithMany()
                        .HasForeignKey("AuthorProfileId", "AuthorDisplayName");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.MessageRevision", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.Message")
                        .WithMany()
                        .HasForeignKey("MessageId");

                    b.HasOne("SOCVR.Slack.StatBot.Database.UserAlias")
                        .WithMany()
                        .HasForeignKey("RevisionAuthorUserId", "RevisionAuthorDisplayName");
                });

            modelBuilder.Entity("SOCVR.Slack.StatBot.Database.ParsedTranscriptPage", b =>
                {
                    b.HasOne("SOCVR.Slack.StatBot.Database.Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
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
