using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;
using MudBlazor;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BackendMaster2.Web.Services;

/// <summary>
/// Portero del cliente HTTP "Api": intercepta cada petición saliente y le mete
/// la cabecera Authorization: Bearer si hay token en la sesión.
/// </summary>

public class AuthDelegatingHandler: DelegatingHandler
{
    private readonly ISessionService _sessionService;
    private readonly IHttpClientFactory _httpClientFactory;
    public AuthDelegatingHandler(ISessionService sessionService, IHttpClientFactory httpClientFactory)
    {
        _sessionService = sessionService;
        _httpClientFactory = httpClientFactory; 
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Intentamos leer el token de la sesión.
        TokenResponseDto? tokens = await _sessionService.GetAsync();

        //si  hay token válido, lo metemos en la cabecera
        if (tokens != null && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        }

        // Continuamos con la petición (con o sin cabecera).
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // Si la respuesta es 401 y tenemos tokens, significa que el access token caducó.        
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized || tokens == null || string.IsNullOrEmpty(tokens.RefreshToken))
        {
            return response;
        }

        // Intentamos hacer refresh del access token usando el refresh token con cliente anónimo.
        HttpClient anonClient = _httpClientFactory.CreateClient("ApiAnonimo");
        using HttpResponseMessage refreshResponse = await anonClient.PostAsJsonAsync("api/auth/refresh", new RefreshRequestDto { RefreshToken = tokens.RefreshToken }, cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            // Refresh falló: sesión muerta. Devolvemos el 401 original y el guard hará su trabajo.
            return response;
        }

        //Guardamos los nuevos tokens en la sesión.
        TokenResponseDto? newTokens = await refreshResponse.Content.ReadFromJsonAsync<TokenResponseDto>(cancellationToken: cancellationToken);
        if (newTokens == null)
        {
            return response;
        }
        await _sessionService.SaveAsync(newTokens);

        // Reconstruir la petición original (no se puede reusar) y reintentar una vez.
        using HttpRequestMessage retryRequest = await ClonarPeticionAsync(request, cancellationToken);
        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);
        // Reintentamos la petición original con el nuevo access token.
        return await base.SendAsync(retryRequest, cancellationToken);


    }

    /// <summary>
    /// Clona un HttpRequestMessage para poder reintentar la petición original.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<HttpRequestMessage>  ClonarPeticionAsync(HttpRequestMessage original, CancellationToken cancellationToken)
    {
        HttpRequestMessage clone = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content != null)
        {
            var bytes = await original.Content.ReadAsByteArrayAsync(cancellationToken);
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in original.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        clone.Version = original.Version;
        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        foreach (var option in original.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        return clone;
    }
}

