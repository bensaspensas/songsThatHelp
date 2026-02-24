namespace SongsThatHelp.Infrastructure.Repositories;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _users = new();

    public User? GetByUsername(string username)
    {
        return _users.TryGetValue(username, out var user) ? user : null;
    }

    public void Add(User user)
    {
        _users[user.Username] = user;
    }

    public void Update(User user)
    {
        _users[user.Username] = user;
    }

    public bool Exists(string username)
    {
        return _users.ContainsKey(username);
    }
}
