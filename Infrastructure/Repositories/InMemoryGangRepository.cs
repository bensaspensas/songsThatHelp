namespace SongsThatHelp.Infrastructure.Repositories;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;

public class InMemoryGangRepository : IGangRepository
{
    private readonly Dictionary<string, Gang> _gangs = new();

    public Gang? GetByName(string name)
    {
        return _gangs.TryGetValue(name, out var gang) ? gang : null;
    }

    public void Add(Gang gang)
    {
        _gangs[gang.Name] = gang;
    }

    public bool Exists(string name)
    {
        return _gangs.ContainsKey(name);
    }
}
