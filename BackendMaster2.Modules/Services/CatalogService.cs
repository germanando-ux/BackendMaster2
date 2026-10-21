using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;


namespace BackendMaster2.Modules.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _CatalogRepository;
    private readonly IMapper _mapper;
    private readonly IAiService _aiService;
    public CatalogService( ICatalogRepository CatalogRepository, IMapper mapper, IAiService aiService)
    {
        _CatalogRepository = CatalogRepository;
        _mapper = mapper;
        _aiService = aiService;
    }

    public async Task<ColorDto> CreateColorAsync(ColorDto colorDto)
    {
        Color newColor = _mapper.Map<Color>(colorDto);
        //genero vector embedding para el nombre del color
        newColor.Embedding = await _aiService.GenerateEmbeddingAsync(newColor.Name);
        await _CatalogRepository.AddColorAsync(newColor);
        return _mapper.Map<ColorDto>(newColor);
        
    }

    public async Task DeleteColorAsync(Guid id)
    {
        var existing = await _CatalogRepository.GetColorByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ningún color con el id {id}.");
        }

        await _CatalogRepository.DeleteColorAsync(id);
    }

    public async Task<IEnumerable<ColorDto>> GetAllColorsAsync(BaseQueryDto query)
    {
        IEnumerable<Color> colors = await _CatalogRepository.GetAllColorsAsync(query);
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

        if (existing is null)
        {
            throw new NotFoundException($"No existe ningún color con el id {color.Id}.");
        }

        existing.Name = color.Name;
        //genero vector embedding para el nombre actualizado
        existing.Embedding = await _aiService.GenerateEmbeddingAsync(existing.Name);
        existing.HexCode = color.HexCode;
        existing.IsActive = color.IsActive;

        await _CatalogRepository.UpdateColorAsync(existing);
        return _mapper.Map<ColorDto>(existing);
    }
}
