namespace SongsThatHelp.Application.Services;

public interface ITokenService
{
    string GenerateToken(string username);
}
