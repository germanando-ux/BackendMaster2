using BackendMaster2.Modules.CatalogManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface IAiService
{
    /// <summary>
    /// Genera y envía el prompt para mejorar la descripción de un producto.
    /// </summary>
    Task<string> EnhanceProductDescriptionAsync(EnhanceProductDescriptionRequestDto enhanceDescriptionRequest,CancellationToken cancellationToken = default);
}
