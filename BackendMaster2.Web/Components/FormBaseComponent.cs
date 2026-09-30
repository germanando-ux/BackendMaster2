using BackendMaster2.Web.Models.Dtos;
using BackendMaster2.Web.Services;
using Microsoft.AspNetCore.Components;

namespace BackendMaster2.Web.Components;

public class FormBaseComponent : ComponentBase
{
    [Inject] protected ErrorHandler ErrorHandler { get; set; } = default!;

    /// <summary>
    /// Id del registro a editar. Si viene null, el formulario está en modo "crear".
    /// Lo inyecta Blazor desde la ruta (ej: /colors/editar/{Id:guid}).
    /// </summary>
    [Parameter] public Guid? Id { get; set; }

    /// <summary>
    /// True si estamos editando un registro existente, False si estamos creando uno nuevo.
    /// </summary>
    protected bool IsEdit => Id.HasValue;
    protected bool IsLoading { get; set; } = true;
    protected bool IsSaving { get; set; } = false;
    protected string? ErrorMessage { get; set; }

    /// <summary>
    /// Maneja un error de API y lo almacena en ErrorMessage.
    /// </summary>
    protected void HandleError<T>(ApiResult<T> result)
    {
        ErrorMessage = ErrorHandler.HandleError(result);
    }
}
