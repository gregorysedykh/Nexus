using Nexus.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Nexus.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Word> Words { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserWord> UserWords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Word>()
                 .HasData(
                new Word("Park", "en") {Id = 1},
                new Word("Haus", "de") {Id = 2});

        modelBuilder.Entity<User>()
                 .HasData(
                new User("john_doe", "john@example.com") { Id = 1 },
                new User("jane_smith", "jane@example.com") { Id = 2 });

        modelBuilder.Entity<UserWord>()
        .HasOne(uw => uw.User)
        .WithMany(u => u.UserWords)
        .HasForeignKey(uw => uw.UserId);

        modelBuilder.Entity<UserWord>()
        .HasOne(uw => uw.Word)
        .WithMany()
        .HasForeignKey(uw => uw.WordId);
    }

    
    
}