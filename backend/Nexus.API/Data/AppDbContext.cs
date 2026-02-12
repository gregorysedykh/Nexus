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
        // Ensure email and username are unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        // Configure composite primary key for UserWord
        modelBuilder.Entity<UserWord>()
            .HasKey(uw => new { uw.UserId, uw.WordId });

        // Seed initial data
        modelBuilder.Entity<Word>()
                 .HasData(
                new Word("Park", "en") {Id = 1},
                new Word("Haus", "de") {Id = 2});

        modelBuilder.Entity<User>()
                 .HasData(
                new User("john_doe", "john@example.com") { Id = 1 },
                new User("jane_smith", "jane@example.com") { Id = 2 });

        // Configure relationships
        // A UserWord has one User and a User can have many UserWords
        modelBuilder.Entity<UserWord>()
        .HasOne(uw => uw.User)
        .WithMany(u => u.UserWords)
        .HasForeignKey(uw => uw.UserId);

        // A UserWord has one Word
        modelBuilder.Entity<UserWord>()
        .HasOne(uw => uw.Word)
        .WithMany()
        .HasForeignKey(uw => uw.WordId);
    }

}
