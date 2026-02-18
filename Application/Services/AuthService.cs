namespace SongsThatHelp.Application.Services;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public bool Register(string username, string password)
    {
        if (_userRepository.Exists(username))
            return false;

        var user = new User(username, password);
        _userRepository.Add(user);
        return true;
    }

    public string? Login(string username, string password)
    {
        var user = _userRepository.GetByUsername(username);
        if (user == null || user.PasswordHash != password)
            return null;

        return _tokenService.GenerateToken(username);
    }
}
