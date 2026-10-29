using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain;

/// <summary>
/// Representa el canal de destino (Outbound): Amazon, eBay, Web Propia, etc.
/// </summary>
public class Marketplace
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: "Amazon ES"
    public string Code { get; set; } = string.Empty; // Ej: "AMAZON_ES"
    public bool IsActive { get; set; } = true;
    // Relación N:M con productos a través de la tabla de unión
    public ICollection<ProductMarketplace> ProductMarketplaces { get; set; } = new List<ProductMarketplace>();
}
