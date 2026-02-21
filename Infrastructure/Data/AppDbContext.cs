namespace SongsThatHelp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using SongsThatHelp.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Song> Songs => Set<Song>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Link).IsRequired();
            entity.Property(e => e.Text).IsRequired();
            entity.OwnsMany(e => e.Comments, comment =>
            {
                comment.Property(c => c.Id);
                comment.Property(c => c.Username).IsRequired();
                comment.Property(c => c.Text).IsRequired();
            });
            entity.OwnsMany(e => e.Emojis, emoji =>
            {
                emoji.Property(e => e.Username).IsRequired();
                emoji.Property(e => e.EmojiType).IsRequired();
            });
        });
    }
}
