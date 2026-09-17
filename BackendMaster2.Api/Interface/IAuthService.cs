using BackendMaster2.Shared.Domain;

namespace BackendMaster2.Api.Interface;

public interface IAuthService
{
    /// <summary>
    /// Comprueba credenciales: devuelve el usuario si todo casa, o null
    /// si falla algo (no existe, contraseña mal o desactivado).
    /// </summary>
    Task<User?> ValidateCredentialsAsync(string email, string password);

    /// <summary>
    /// Crea el access token: un JSON firmado con nuestra clave que dice
    /// quién es el usuario, qué rol tiene y cuándo caduca.
    /// </summary>
    (string Token, DateTime ExpiresAt) CreateAccessToken(User user);

    /// <summary>
    /// Inventa un refresh token aleatorio, guarda SOLO su hash en BD
    /// y devuelve el original en claro (único sitio donde existe sin hashear).
    /// </summary>
    Task<string> CreateRefreshTokenAsync(Guid userId);

    /// <summary>
    /// Comprueba un refresh token recibido. Devuelve el usuario si pasa
    /// las cuatro pruebas (existe, no revocado, no caducado, usuario activo)
    /// o null si falla cualquiera.
    /// </summary>
    Task<User?> ValidateRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Logout: marca la sesión como revocada. Con esto el refresh muere
    /// aunque alguien tuviera una copia.
    /// </summary>
    Task RevokeRefreshTokenAsync(string refreshToken);
}
