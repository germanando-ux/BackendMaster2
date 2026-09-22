using BackendMaster2.Web.Interfaces;

namespace BackendMaster2.Web.Services;

/// <summary>
/// Portero del cliente HTTP "Api": intercepta cada petición saliente y le mete
/// la cabecera Authorization: Bearer si hay token en la sesión.
/// </summary>

public class AuthDelegatingHandler: DelegatingHandler
{
    private readonly ISessionService _sessionService;
    public AuthDelegatingHandler(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Intentamos leer el token de la sesión.
        var tokens = await _sessionService.GetAsync();

        //si  hay token válido, lo metemos en la cabecera
        if (tokens != null && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        }


        // Continuamos con la petición (con o sin cabecera).
        return await base.SendAsync(request, cancellationToken);
    }


}
