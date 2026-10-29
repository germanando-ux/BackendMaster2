using System;

namespace BackendMaster2.Shared.Domain;

/// <summary>
/// Representa el origen de los datos (Inbound): ERP, Shopify, CSV, Manual, etc.
/// </summary>
public class DataSource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: "Shopify Tienda ES"
    public string Code { get; set; } = string.Empty; // Ej: "SHOPIFY_ES" (para lógica interna)
    public bool IsActive { get; set; } = true;
    // Relación N:M con productos a través de la tabla de unión
    public ICollection<ProductDataSource> ProductDataSources { get; set; } = new List<ProductDataSource>();
}