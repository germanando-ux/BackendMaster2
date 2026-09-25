using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain
{

    /// <summary>
    /// Entidad central del PIM. Compartida porque la usan
    /// ProductManagement (CRUD) y ProductSync (sincronización).
    /// </summary>
    public class Product
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;      // Identificador único para marketplaces
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;           // Permite desactivar sin borrar
        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
    }
}
