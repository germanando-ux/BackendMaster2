using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IApiClient
{
    Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request);
    Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body = null);
}
}
