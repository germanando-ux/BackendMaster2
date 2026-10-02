namespace BackendMaster2.Web.Services;

public class ToastService
{

   // Propiedad pública de solo lectura para que la UI pueda ver los mensajes.
    public IReadOnlyList<string> Mensajes => _messages.AsReadOnly();

    // Lista privada donde guardaremos los mensajes.
    private readonly List<string> _messages = new();

    /// <summary>
    /// Método para añadir un mensaje de error a la lista.
    /// </summary>
    public void ShowError(string message)
    {
        _messages.Add(message);
        NotifyStateChanged(); // ¡Avisamos a la UI de que hay un mensaje nuevo!
        // Disparamos un temporizador en segundo plano para borrar el mensaje
        _ = Task.Run(async () =>
        {
            await Task.Delay(4000); // Espera 4 segundos
            _messages.Remove(message);
            NotifyStateChanged();
        });
    }

    // Evento que avisa a los componentes cuando la lista de mensajes cambia.
    public event Action? OnChange;
    // Método interno para disparar el evento.
    private void NotifyStateChanged() => OnChange?.Invoke();
}
