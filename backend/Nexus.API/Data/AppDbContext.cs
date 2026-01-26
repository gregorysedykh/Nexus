using Nexus.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Nexus.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Word> Words { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Word>()
                 .HasData(
                new Word("Park", "en") {Id = 1},
                new Word("Haus", "de") {Id = 2});
    }
    
}