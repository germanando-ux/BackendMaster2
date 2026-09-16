using BackendMaster2.Modules.Auth.Interface;
using BackendMaster2.Modules.Data;
using BackendMaster2.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace BackendMaster2.Modules.Auth.Data;

/// <summary>
/// El único sitio de la solución que sabe cómo se consultan usuarios
/// y refresh tokens. Si mañana cambia el almacenamiento, solo cambia esta clase.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}
