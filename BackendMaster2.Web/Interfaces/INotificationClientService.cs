using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Web.Interfaces;

public interface INotificationClientService : IAsyncDisposable
{
    /// <summary>
    /// Inicia la conexión con el Hub de notificaciones.
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Evento que se dispara cuando la IA termina de procesar una tarea.
    /// Parámetros: TaskId, Resultado (texto).
    /// </summary>
    event Action<string>? OnTaskCompleted;

    /// <summary>
    /// Evento que se dispara cuando hay un error en el procesamiento.
    /// Parámetros: TaskId, Mensaje de error.
    /// </summary>
    event Action<string, string>? OnTaskFailed;

    /// <summary>
    /// Devuelve el ConnectionId actual para pasárselo al backend.
    /// </summary>
    string? GetConnectionId();
}