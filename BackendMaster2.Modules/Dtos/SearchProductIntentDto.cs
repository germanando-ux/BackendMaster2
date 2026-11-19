using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;



/// <summary>
/// Dto en el que la IA "serializa" el texto que el usuario ha escrito para buscar un producto y lo convierte en filtros de búsqueda concretos.
/// </summary>
public class SearchProductIntentDto
{
    
    // Texto limpio para buscar semánticamente en la descripción o nombre del producto.
    // Ejemplo: "zapatillas para correr maratón"
    
    public string ProductQuery { get; set; } = string.Empty;

    
    // Texto limpio para buscar semánticamente en el nombre del color.
    // Ejemplo: "burdeos" o "verde aguamarina"
    public string ColorQuery { get; set; } = string.Empty;

    // Filtro duro de precio máximo. Null si el usuario no mencionó ningún límite de precio.
    public decimal? MinPrice { get; set; }

    // Filtro duro de precio máximo. Null si el usuario no mencionó ningún límite de precio.
    public decimal? MaxPrice { get; set; }
    public List<string>? SizeNames { get; set; }
    // Filtro duro de stock mínimo. Por defecto 1 (solo mostrar productos disponibles).
    public int? MinStock { get; set; } = 1;
}
