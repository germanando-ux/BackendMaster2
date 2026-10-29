using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared;

public class ProductDataSource
{
    // Clave compuesta (PK y FK a la vez)
    public Guid ProductId { get; set; }
    public Guid DataSourceId { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public DateTime? LastSyncedAt { get; set; }

    // Navegaciones (para EF Core)
    public Product Product { get; set; } = null!;
    public DataSource DataSource { get; set; } = null!;
}
