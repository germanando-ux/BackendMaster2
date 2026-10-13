using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackendMaster2.Web.Services;

public class ToastService
{

    public record ToastItem(string Message, bool IsError);

    // Propiedad pública de solo lectura para que la UI pueda ver los mensajes.
    public IReadOnlyList<ToastItem> Mensajes => _messages.AsReadOnly();

    // Lista privada donde guardaremos los mensajes.
    private readonly List<ToastItem> _messages = new();

    public void ShowError (string message) => AddMessage(message, true);
    public void ShowInfo(string message) => AddMessage(message, false);


    /// <summary>
    /// Método para añadir un mensaje de error a la lista.
    /// </summary>
    public void AddMessage(string message, bool isError)
    {
        var item = new ToastItem(message, isError);
        _messages.Add(item);
        NotifyStateChanged(); // ¡Avisamos a la UI de que hay un mensaje nuevo!
        // Disparamos un temporizador en segundo plano para borrar el mensaje
        _ = Task.Run(async () =>
        {
            await Task.Delay(4000); // Espera 4 segundos
            _messages.Remove(item);
            NotifyStateChanged();
        });
    }

    public void HideMessage()
    {
        _messages.RemoveAll(m => !m.IsError);
        NotifyStateChanged();
    }

    // Evento que avisa a los componentes cuando la lista de mensajes cambia.
    public event Action? OnChange;
    // Método interno para disparar el evento.
    private void NotifyStateChanged() => OnChange?.Invoke();
}
