using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;
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

    public async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }
        return await SendAsync<T>(request);
    }

    public async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request)
    {
        using var response = await _http.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            return ApiResult<T>.Ok(data!, response.StatusCode);
        }

        // Error: intentar leer el ProblemDetails del body. Si no hay body o no es JSON válido, error genérico.
        ApiProblemDetailsDto? error = null;
        try
        {
            error = await response.Content.ReadFromJsonAsync<ApiProblemDetailsDto>(JsonOptions);
        }
        catch
        {
            // No es un ProblemDetails válido: seguimos con error null y solo StatusCode.
        }
        return ApiResult<T>.Fail(error, response.StatusCode);
    }
}