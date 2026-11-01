using Microsoft.EntityFrameworkCore;    
using P2_RaceDay.API.Models;

namespace P2_RaceDay.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<User> Users => Set<User>();
     public DbSet<Category> Categories => Set<Category>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<RaceEntry> RaceEntries => Set<RaceEntry>();
    public DbSet<RaceResult> Results => Set<RaceResult>();
   




}

