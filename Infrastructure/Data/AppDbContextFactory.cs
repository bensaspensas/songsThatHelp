namespace SongsThatHelp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a dummy connection string for migrations
        // The actual connection string will be used at runtime
        optionsBuilder.UseNpgsql("Host=localhost;Database=songs;Username=postgres;Password=postgres");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
