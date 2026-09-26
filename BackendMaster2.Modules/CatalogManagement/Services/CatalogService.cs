using BackendMaster2.Modules.CatalogManagement.Interfaces;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Shared.Domain;


namespace BackendMaster2.Modules.CatalogManagement.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _CatalogRepository;
    public CatalogService( ICatalogRepository CatalogRepository)
    {
        _CatalogRepository = CatalogRepository;
    }

    public async Task<IEnumerable<Color>> GetAllColorsAsync()
    {
        return await _CatalogRepository.GetAllColorsAsync();
    }
    public async Task<Color> GetColorByIdAsync(Guid id)
    {
        return await _CatalogRepository.GetColorByIdAsync(id);
    }
}
