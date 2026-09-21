using BackendMaster2.Web.Models.Dtos;
using Microsoft.JSInterop;

namespace BackendMaster2.Web.Services;

/// <summary>
/// Almacén de la sesión del usuario: guarda los dos tokens en localStorage
/// y los devuelve cuando hace falta. Todo el acceso a JS queda encapsulado aquí.
/// </summary>
public class SessionService
{ 
    private readonly IJSRuntime _js;
    private const string StorageKey =  "backendmaster2.session";

    public SessionService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Guarda los dos tokens y su caducidad en localStorage.
    /// </summary>
    public async Task SaveAsync(TokenResponseDto tokens)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, System.Text.Json.JsonSerializer.Serialize(tokens));
    }


    /// <summary>
    /// Lee los tokens guardados. Devuelve null si no hay sesión o están caducados.
    /// </summary>
    public async Task<TokenResponseDto?> GetAsync()
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }
        var tokens = System.Text.Json.JsonSerializer.Deserialize<TokenResponseDto>(json);
        if (tokens == null)
        {
            return null;
        }

        // Si el access token ya caducó, tratamos como si no hubiera sesión.
        if (tokens.AccessTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            return null;
        }
        return tokens;

    }

    /// <summary>
    /// Borra la sesión (logout).
    /// </summary>
    public async Task BorrarAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }

}
