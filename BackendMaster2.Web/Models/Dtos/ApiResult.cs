using System.Net;

namespace BackendMaster2.Web.Models.Dtos;

/// <summary>
/// Resultado uniforme de una llamada a la API. Nunca null, siempre explícito.
/// </summary>
public class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public T? Data { get; init; }
    public ApiProblemDetailsDto? Error { get; init; }

    public static ApiResult<T> Ok(T data, HttpStatusCode status = HttpStatusCode.OK) => new() { IsSuccess = true, Data = data, StatusCode = status };

    public static ApiResult<T> Fail(ApiProblemDetailsDto? error, HttpStatusCode status) => new() { IsSuccess = false, Error = error, StatusCode = status };
}