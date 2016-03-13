using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace SOCVR.Slack.StatBot.Database
{
    class MessageStorage : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ParsedTranscriptPage> ParsedTranscriptPages { get; set; }
        public DbSet<MessageRevision> MessageRevisions { get; set; }
        public DbSet<UserAlias> UserAliases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = SettingsAccessor.GetSetting<string>("ConnectionString");
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureMessageEntity(modelBuilder);
            ConfigureMessageRevisionEntity(modelBuilder);
            ConfigureParsedTranscriptPageEntity(modelBuilder);
            ConfigureRoomEntity(modelBuilder);
            ConfigureUserEntity(modelBuilder);
            ConfigureUserAliasEntity(modelBuilder);
        }

        private static void ConfigureMessageEntity(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<Message>()
                .HasKey(x => x.MessageId);

            //value generation
            modelBuilder.Entity<Message>()
                .Property(x => x.MessageId)
                .ValueGeneratedNever();

            //foreign keys
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Author)
                .WithMany(a => a.Messages)
                .HasForeignKey(m => m.AuthorId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId);
        }

        private static void ConfigureMessageRevisionEntity(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<MessageRevision>()
                .HasKey(x => new { x.MessageId, x.RevisionNumber });

            //field properties
            modelBuilder.Entity<MessageRevision>()
                .Property(x => x.Text)
                .IsRequired();

            //foreign keys
            modelBuilder.Entity<MessageRevision>()
                .HasOne(mr => mr.Message)
                .WithMany(m => m.MessageRevisions)
                .HasForeignKey(m => m.MessageId);

            modelBuilder.Entity<MessageRevision>()
                .HasOne(mr => mr.RevisionAuthor)
                .WithMany(a => a.MessageRevisions)
                .HasForeignKey(mr => mr.RevisionAuthorId);
        }

        private static void ConfigureParsedTranscriptPageEntity(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<ParsedTranscriptPage>()
                .HasKey(x => new { x.RoomId, x.Date });

            //foreign keys
            modelBuilder.Entity<ParsedTranscriptPage>()
                .HasOne(p => p.Room)
                .WithMany(r => r.ParsedTranscriptPages)
                .HasForeignKey(p => p.RoomId);
        }

        private static void ConfigureRoomEntity(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<Room>()
                .HasKey(x => x.RoomId);

            //field properties
            modelBuilder.Entity<Room>()
                .Property(x => x.RoomId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Room>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<Room>()
                .Property(x => x.Description)
                .IsRequired();
        }

        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<User>()
                .HasKey(x => x.ProfileId);

            //field properties
            modelBuilder.Entity<User>()
                .Property(x => x.ProfileId)
                .ValueGeneratedNever();

            //JoinedChatSystemAt is always going to be a date, but C# doesn't have a good
            //"date only" data type that is also compatable with EF. DateTimeOffset is going to be
            //a bit overkill, but should be fine.
        }

        private static void ConfigureUserAliasEntity(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<UserAlias>()
                .HasKey(x => new { x.UserId, x.DisplayName });

            //foreign keys
            modelBuilder.Entity<UserAlias>()
                .HasOne(a => a.User)
                .WithMany(u => u.Aliases)
                .HasForeignKey(a => a.UserId);
        }
    }
}
