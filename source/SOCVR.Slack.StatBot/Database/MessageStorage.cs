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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = SettingsAccessor.GetSetting<string>("ConnectionString");
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //message
            modelBuilder.Entity<Message>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Message>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.PostedMessages)
                .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId);

            //room
            modelBuilder.Entity<Room>()
                .Property(x => x.RoomId)
                .ValueGeneratedNever();

            //user
            modelBuilder.Entity<User>()
                .HasKey(x => x.ProfileId);

            modelBuilder.Entity<User>()
                .Property(x => x.ProfileId)
                .ValueGeneratedNever();
        }
    }
}
