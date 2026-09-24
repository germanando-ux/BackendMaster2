using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BackendMaster2.Web.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;

    // Opciones JSON compartidas: inmutables y thread-safe. Se crean una vez.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// </summary>
    /// Envía una petición HTTP a partir del verbo, la URL relativa y un cuerpo opcional,
    /// y devuelve el resultado normalizado como <see cref="ApiResult{T}"/>.
    /// Es la vía habitual: quien llama no necesita construir un <see cref="HttpRequestMessage"/>.
    /// Nunca lanza por errores HTTP o de red: los traduce a un resultado con IsSuccess = false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        //enpoint para simular error
        //using var request = new HttpRequestMessage(method, "api/products/error-plano");
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }
        return await SendRequestAsync<T>(request);
    }
    /// <summary>
    /// /// Envía un <see cref="HttpRequestMessage"/> ya construido por el llamante y devuelve
    /// el resultado normalizado como <see cref="ApiResult{T}"/>.
    /// Vía de escape para casos raros que exigen control total de la petición
    /// (cabeceras personalizadas, contenido no JSON). El resto de llamadas usa la sobrecarga sencilla.

    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResult<T>> SendRequestAsync<T>(HttpRequestMessage request)
    {

        //primera parte obtenemos el response, si no hay como no llega nada, entonces devolvemos un error 503 con formato de problem details
        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            // La API no contestó: no hay status HTTP real, sintetizamos un 503.
            return ApiResult<T>.Fail(new ApiProblemDetailsDto
            {
                Title = "Sin conexión con la API",
                Detail = ex.Message
            }, HttpStatusCode.ServiceUnavailable);
        }

        //evaluamos el response, si es exitoso devolvemos el data, si no es exitoso intentamos leer el problem details del body, si no hay devolvemos un error generico
        using (response)
        {
            if (response.IsSuccessStatusCode)
            {
                T? data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
                return ApiResult<T>.Ok(data!, response.StatusCode);
            }

            // Error: intentar leer el ProblemDetails del body. Si no hay body o no es JSON válido, error genérico.
            ApiProblemDetailsDto? error = null;
            try
            {
                error = await response.Content.ReadFromJsonAsync<ApiProblemDetailsDto>(JsonOptions);
            }
            //Canaliza los errores no controlados por middleware de errores de la api, sin formato ApiProblemDetails para que no se eleven
            catch (JsonException)
            {
                // No es un ProblemDetails válido: seguimos con error null y solo StatusCode.
            }
            return ApiResult<T>.Fail(error, response.StatusCode);

        }
    }
}