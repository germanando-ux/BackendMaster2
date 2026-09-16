using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain
{
    /// <summary>
    /// Ficha de datos personales del usuario (perfil de dominio).
    /// Separada de User: identidad y datos personales tienen ciclos de vida
    /// y trato regulatorio (RGPD) distintos.
    /// </summary>
    public class Person
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }              // 1-a-1: la persona es el perfil de esta cuenta
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }               // navegación para el alta conjunta del perforador
    }
}
