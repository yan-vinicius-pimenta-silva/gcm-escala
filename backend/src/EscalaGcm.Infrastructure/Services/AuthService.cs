using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EscalaGcm.Application.DTOs.Auth;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EscalaGcm.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.NomeUsuario == request.NomeUsuario && u.Ativo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return null;

        var token = GenerateJwtToken(usuario.Id, usuario.NomeUsuario, usuario.Perfil.ToString());

        return new LoginResponse(token, usuario.NomeCompleto, usuario.Perfil.ToString());
    }

    private string GenerateJwtToken(int userId, string username, string perfil)
    {
        // REVIEW: Same fallback key duplicated from Program.cs. If one changes, token validation breaks silently.
        // Extract to a shared constant or inject the already-resolved key.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "EscalaGcmSuperSecretKeyForDevelopment2024!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, perfil)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "EscalaGcm",
            audience: _configuration["Jwt:Audience"] ?? "EscalaGcm",
            claims: claims,
            // REVIEW: Token lifetime hardcoded to 8h. Make configurable via appsettings.
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
