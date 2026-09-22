using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface ISessionService
{
    /// <summary>
    /// Guarda los dos tokens y su caducidad en localStorage.
    /// </summary>
    Task SaveAsync(TokenResponseDto tokens);

    /// <summary>
    /// Lee los tokens guardados. Devuelve null si no hay sesión o están caducados.
    /// </summary>
    Task<TokenResponseDto?> GetAsync();

    /// <summary>
    /// Borra la sesión (logout).
    /// </summary>
    Task DeleteAsync();
}