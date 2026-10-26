using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class SearchProductIntentDto
{
    
    // Texto limpio para buscar semánticamente en la descripción o nombre del producto.
    // Ejemplo: "zapatillas para correr maratón"
    
    public string ProductQuery { get; set; } = string.Empty;

    
    // Texto limpio para buscar semánticamente en el nombre del color.
    // Ejemplo: "burdeos" o "verde aguamarina"
    public string ColorQuery { get; set; } = string.Empty;

    
    // Filtro duro de precio máximo. Null si el usuario no mencionó ningún límite de precio.
    public decimal? MaxPrice { get; set; }

    
    // Filtro duro de stock mínimo. Por defecto 1 (solo mostrar productos disponibles).
    
    public int? MinStock { get; set; } = 1;
}
