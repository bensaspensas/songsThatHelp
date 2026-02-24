namespace SongsThatHelp.Application.Services;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGangRepository _gangRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IGangRepository gangRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _gangRepository = gangRepository;
        _tokenService = tokenService;
    }

    public bool CreateGang(string gangName, string gangPassword)
    {
        if (_gangRepository.Exists(gangName))
            return false;

        var gang = new Gang(gangName, gangPassword);
        _gangRepository.Add(gang);
        return true;
    }

    public bool Register(string username, string password, string? gangName = null, string? gangPassword = null)
    {
        if (_userRepository.Exists(username))
            return false;

        // If gang is specified, validate it
        if (!string.IsNullOrEmpty(gangName))
        {
            var gang = _gangRepository.GetByName(gangName);
            if (gang == null || gang.PasswordHash != gangPassword)
                return false;
        }

        var user = new User(username, password, gangName);
        _userRepository.Add(user);
        return true;
    }

    public (string? token, string? gangName) Login(string username, string password, string? gangName = null, string? gangPassword = null)
    {
        var user = _userRepository.GetByUsername(username);
        if (user == null || user.PasswordHash != password)
            return (null, null);

        // If gang is specified in login, validate and update user's gang
        if (!string.IsNullOrEmpty(gangName))
        {
            var gang = _gangRepository.GetByName(gangName);
            if (gang == null || gang.PasswordHash != gangPassword)
                return (null, null);
            
            // Update user's gang if different
            if (user.GangName != gangName)
            {
                user.GangName = gangName;
                _userRepository.Update(user);
            }
        }

        var token = _tokenService.GenerateToken(username, user.GangName);
        return (token, user.GangName);
    }
}
