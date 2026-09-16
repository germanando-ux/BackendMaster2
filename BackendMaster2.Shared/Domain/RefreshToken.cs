using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain
{
    /// <summary>
    /// Sesión revocable. Guardamos el HASH (SHA-256) del token, nunca el token:
    /// si la BD se filtra, estos registros no sirven para entrar.
    /// </summary>
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;  // SHA-256 hex del token entregado al cliente
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAtUtc { get; set; }            // null = sesión viva

        public User? User { get; set; }
    }
}
