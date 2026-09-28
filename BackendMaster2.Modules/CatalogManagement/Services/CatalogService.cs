using AutoMapper;
using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Modules.CatalogManagement.Interfaces;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;


namespace BackendMaster2.Modules.CatalogManagement.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _CatalogRepository;
    private readonly IMapper _mapper;
    public CatalogService( ICatalogRepository CatalogRepository, IMapper mapper)
    {
        _CatalogRepository = CatalogRepository;
        _mapper = mapper;
    }

    public async Task<ColorDto> CreateColorAsync(ColorDto colorDto)
    {
        Color newColor = _mapper.Map<Color>(colorDto);
        await _CatalogRepository.AddColorAsync(newColor);
        return _mapper.Map<ColorDto>(newColor);
    }

    public async Task<IEnumerable<ColorDto>> GetAllColorsAsync()
    {
        IEnumerable<Color> colors = await _CatalogRepository.GetAllColorsAsync();
        return colors.Select(c => _mapper.Map<ColorDto>(c));
    }
    public async Task<ColorDto?> GetColorByIdAsync(Guid id)
    {
        Color color = await _CatalogRepository.GetColorByIdAsync(id);
        return color != null ? _mapper.Map<ColorDto>(color) : null;
    }

    public async Task<ColorDto?> UpdateColorAsync(ColorDto color)
    {
        // 1. Cargo la entidad RASTREADA (la que vive en la memoria del DbContext).
        Color? existing = await _CatalogRepository.GetColorByIdAsync(color.Id);
        existing.Name = color.Name;
        existing.HexCode = color.HexCode;
        existing.IsActive = color.IsActive;

        await _CatalogRepository.UpdateColorAsync(existing);
        return _mapper.Map<ColorDto>(existing);
    }
}
