namespace SongsThatHelp.Infrastructure.Repositories;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;
using SongsThatHelp.Infrastructure.Data;

public class EfGangRepository : IGangRepository
{
    private readonly AppDbContext _context;

    public EfGangRepository(AppDbContext context)
    {
        _context = context;
    }

    public Gang? GetByName(string name)
    {
        return _context.Gangs.Find(name);
    }

    public void Add(Gang gang)
    {
        _context.Gangs.Add(gang);
        _context.SaveChanges();
    }

    public bool Exists(string name)
    {
        return _context.Gangs.Any(g => g.Name == name);
    }
}
