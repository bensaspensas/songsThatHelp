namespace SongsThatHelp.Domain.Entities;

public class Gang
{
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public Gang() { }

    public Gang(string name, string passwordHash)
    {
        Name = name;
        PasswordHash = passwordHash;
    }
}
