namespace SongsThatHelp.Application.Services;

public interface IAuthService
{
    bool Register(string username, string password, string? gangName = null, string? gangPassword = null);
    (string? token, string? gangName) Login(string username, string password, string? gangName = null, string? gangPassword = null);
    bool CreateGang(string gangName, string gangPassword);
}

