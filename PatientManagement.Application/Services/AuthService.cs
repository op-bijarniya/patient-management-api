using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Interfaces;

namespace PatientManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(command.Username, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new AuthenticationException("Invalid username or password.");
        }

        var token = GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
        };
    }

    private string GenerateToken(Domain.Entities.User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
