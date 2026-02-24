namespace SongsThatHelp.Infrastructure.Repositories;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;
using SongsThatHelp.Infrastructure.Data;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
    {
        return _context.Users.Find(username);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public bool Exists(string username)
    {
        return _context.Users.Any(u => u.Username == username);
    }
}
