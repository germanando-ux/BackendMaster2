using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;

public class DataSourceClient : IDataSourceClient
{
    private readonly IApiClient _apiClient;
    public DataSourceClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResult<DataSourceDto>> CreateDataSourceAsync(DataSourceDto dataSource)
    {
        return await _apiClient.SendAsync<DataSourceDto>(HttpMethod.Post, $"api/masterdata/CreateDataSource", dataSource);
    }

    public async Task<ApiResult<bool>> DeleteDataSourceAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/masterdata/DeleteDataSource/{id}");
    }

    public async Task<ApiResult<List<DataSourceDto>>> GetAllDataSourceAsync(BaseQueryDto? query = null)
    {
        string urlParameters = _apiClient.CreateUrlParameters(query);
        return await _apiClient.SendAsync<List<DataSourceDto>>(HttpMethod.Get, $"api/masterdata/GetAllDataSources{urlParameters}");
    }

    public async Task<ApiResult<DataSourceDto>> GetDataSourceByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<DataSourceDto>(HttpMethod.Get, $"api/masterdata/GetDataSourceById/{id}");
    }

    public async Task<ApiResult<DataSourceDto>> UpdateDataSourceAsync(Guid id, DataSourceDto dataSource)
    {
        return await _apiClient.SendAsync<DataSourceDto>(HttpMethod.Put, $"api/masterdata/UpdateDataSource/{id}", dataSource);
    }
}
