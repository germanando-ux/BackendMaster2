using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public class ProductFeatureDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid FeatureId { get; set; }

    // Nombre de la feature (para mostrar en el front, mapeado desde la navegación)
    public string FeatureName { get; set; } = string.Empty;

    // NUEVO: Opción seleccionada si es tipo Lista
    public Guid? FeatureOptionId { get; set; }
    public string? FeatureOptionValue { get; set; } // Para mostrar el valor de la opción en el front

    // Valor libre (para tipos Texto, Número, etc.)
    public string? Value { get; set; }

    public Guid? ChannelId { get; set; }
}