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

    public DbSet<RaceEvents> Events => Set<RaceEvents>();
    public DbSet<User> Users => Set<User>();
     public DbSet<Category> Categories => Set<Category>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<RaceEntry> RaceEntries => Set<RaceEntry>();
   




}

