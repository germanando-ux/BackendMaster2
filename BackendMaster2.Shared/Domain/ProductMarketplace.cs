using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain;

public class ProductMarketplace
{
    // Clave compuesta (PK y FK a la vez)
    public Guid ProductId { get; set; }
    public Guid MarketplaceId { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public DateTime? LastSyncedAt { get; set; }

    // Navegaciones (para EF Core)
    public Product Product { get; set; } = null!;
    public Marketplace Marketplace { get; set; } = null!;
}
