using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain
{
    /// <summary>
    /// Cuenta de usuario: identidad y autorización.
    /// La contraseña nunca se guarda en claro: solo su hash BCrypt con salt.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";   // "User" | "Admin": alimentará el claim role
        public bool IsActive { get; set; } = true;   // desactivado: no emite tokens nuevos
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Person? Person { get; set; }          // ficha de datos personales (1-a-1)
    }
}
