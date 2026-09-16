using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Auth.Interface;

/// <summary>
/// Acceso a datos de usuarios y sesiones.
/// El service pregunta en idioma de negocio; el repository traduce a base de datos.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
}
