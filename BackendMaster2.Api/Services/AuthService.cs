using BackendMaster2.Api.Interface;
using BackendMaster2.Api.Settings;
using BackendMaster2.Modules.Auth.Data;
using BackendMaster2.Modules.Auth.Interface;
using BackendMaster2.Modules.Data;
using BackendMaster2.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackendMaster2.Api.Services;
/// <summary>
/// Fábrica y guardián de tokens: firma access tokens (JWT) y gestiona
/// refresh tokens hasheados en base de datos.
/// </summary>
public class AuthService: IAuthService
{
    private readonly JwtSettings _jwt;
    private readonly IUserRepository _userRepository;


    public AuthService(IOptions<JwtSettings> jwt, IUserRepository userRepository)
    {
        _jwt = jwt.Value;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Comprueba credenciales: devuelve el usuario si todo casa, o null
    /// si falla algo (no existe, contraseña mal o desactivado).
    /// Los tres fallos devuelven el mismo null a propósito.
    /// </summary>
    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        if (!user.IsActive) return null;

        return user;
    }

    /// <summary>
    /// Crea el access token: un JSON firmado con nuestra clave que dice
    /// quién es el usuario, qué rol tiene y cuándo caduca.
    /// </summary>
    public (string Token, DateTime ExpiresAt) CreateAccessToken(User user)
    {
        // Ahora mismo más lo que diga el appsettings (15 min).
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);

        // Los claims: los datos que viajan DENTRO del token.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),   // quién es
            new Claim("role", user.Role),                         // qué puede hacer
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // DNI del token: único siempre
        };

        // La clave que firma: el texto del appsettings convertido a bytes.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // El token: quién lo emite, para quién es, qué dice, cuándo caduca y la firma.
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        // Convierte el objeto token en el texto largo que viajará por el cable.
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    /// <summary>
    /// Inventa un refresh token aleatorio, guarda SOLO su hash en BD
    /// y devuelve el original en claro (único sitio donde existe sin hashear).
    /// </summary>
    public async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var randomBytes = new byte[32];
        RandomNumberGenerator.Fill(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);

        await _userRepository.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = HashToken(refreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
        });

        return refreshToken;
    }

    /// <summary>
    /// Comprueba un refresh token recibido. Devuelve el usuario si pasa
    /// las cuatro pruebas (existe, no revocado, no caducado, usuario activo)
    /// o null si falla cualquiera.
    /// </summary>
    public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _userRepository.FindRefreshTokenByHashAsync(HashToken(refreshToken));


        if (storedToken == null) return null;        
        if (storedToken.RevokedAtUtc != null) return null;
        if (storedToken.ExpiresAtUtc < DateTime.UtcNow) return null;
        if (!storedToken.User?.IsActive != true) return null;

        return storedToken.User;
    }

    /// <summary>
    /// Logout: marca la sesión como revocada. Con esto el refresh muere
    /// aunque alguien tuviera una copia.
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _userRepository
            .FindRefreshTokenByHashAsync(HashToken(refreshToken));

        if (storedToken != null && storedToken.RevokedAtUtc == null)
        {
            storedToken.RevokedAtUtc = DateTime.UtcNow;
            await _userRepository.UpdateRefreshTokenAsync(storedToken);
        }
    }


    /// <summary>
    /// Genera  un hash 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();
    }
}

