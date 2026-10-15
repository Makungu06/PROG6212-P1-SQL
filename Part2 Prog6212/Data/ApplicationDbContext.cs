using Microsoft.EntityFrameworkCore;
using Part2_Prog6212.Models;

namespace Part2_Prog6212.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<RaceEvent> RaceEvents { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<RaceEntry> RaceEntries { get; set; }
        public DbSet<RaceResult> RaceResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                 base.OnModelCreating(modelBuilder);

                 modelBuilder.Entity<User>()
                    .HasIndex(u =>u.Email)
                    .IsUnique();
                 

                 modelBuilder.Entity<Participant>()
                    .HasOne(p => p.User)
                    .WithOne()
                    .HasForeignKey<Participant>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);


                  modelBuilder.Entity<Category>()
                    .HasOne(c => c.RaceEvent)
                    .WithMany(e => e.Categories)
                    .HasForeignKey(c => c.RaceEventId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Race>()
              .HasOne(r => r.RaceEvent)
              .WithMany(e => e.Races)
              .HasForeignKey(r => r.RaceEventId);
              

            modelBuilder.Entity<RaceEntry>()
                .HasOne(e => e.Participant)
                .WithMany(p => p.RaceEntries)
                .HasForeignKey(e => e.ParticipantId);

            modelBuilder.Entity<RaceEntry>()
                .HasOne(e => e.Race)
                .WithMany(r => r.RaceEntries)
                .HasForeignKey(e => e.RaceId);

            modelBuilder.Entity<RaceEntry>()
                .HasIndex(e => new { e.ParticipantId, e.RaceId })
                .IsUnique();

            modelBuilder.Entity<RaceEntry>()
                .HasOne(r => r.RaceEntryId)
                .WithOne(e => e.Result)
                .HasForeignKey<RaceResult>(r => r.RaceEntryId);

            modelBuilder.Entity<Race>()
                .Property(r => r.DistanceKm)
                .HasPrecision(8, 2);



        }


    }
}
