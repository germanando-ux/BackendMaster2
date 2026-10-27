using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IDataSourceClient
{
    Task<ApiResult<List<DataSourceDto>>> GetAllDataSourceAsync(BaseQueryDto? query = null);
    Task<ApiResult<DataSourceDto>> GetDataSourceByIdAsync(Guid id);
    Task<ApiResult<DataSourceDto>> CreateDataSourceAsync(DataSourceDto dataSource);
    Task<ApiResult<DataSourceDto>> UpdateDataSourceAsync(Guid id, DataSourceDto dataSource);
    Task<ApiResult<bool>> DeleteDataSourceAsync(Guid id);
}
