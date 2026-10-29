using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Modules.Repository;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;


namespace BackendMaster2.Modules.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IMapper _mapper;
    private readonly IAiService _aiService;
    public CatalogService( ICatalogRepository CatalogRepository, IMapper mapper, IAiService aiService)
    {
        _catalogRepository = CatalogRepository;
        _mapper = mapper;
        _aiService = aiService;
    }


    //colores
    public async Task<ColorDto> CreateColorAsync(ColorDto colorDto)
    {
        Color newColor = _mapper.Map<Color>(colorDto);
        //genero vector embedding para el nombre del color
        newColor.Embedding = await _aiService.GenerateEmbeddingAsync(newColor.Name);
        await _catalogRepository.CreateColorAsync(newColor);
        return _mapper.Map<ColorDto>(newColor);
        
    }

    public async Task DeleteColorAsync(Guid id)
    {
        var existing = await _catalogRepository.GetColorByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ningún color con el id {id}.");
        }

        await _catalogRepository.DeleteColorAsync(id);
    }

    public async Task<IEnumerable<ColorDto>> GetAllColorsAsync(BaseQueryDto query)
    {
        IEnumerable<Color> colors = await _catalogRepository.GetAllColorsAsync(query);
        return colors.Select(c => _mapper.Map<ColorDto>(c));
    }
    public async Task<ColorDto?> GetColorByIdAsync(Guid id)
    {
        Color color = await _catalogRepository.GetColorByIdAsync(id);
        return color != null ? _mapper.Map<ColorDto>(color) : null;
    }

    public async Task<ColorDto?> UpdateColorAsync(ColorDto color)
    {
        // 1. Cargo la entidad RASTREADA (la que vive en la memoria del DbContext).
        Color? existing = await _catalogRepository.GetColorByIdAsync(color.Id);

        if (existing is null)
        {
            throw new NotFoundException($"No existe ningún color con el id {color.Id}.");
        }

        existing.Name = color.Name;
        //genero vector embedding para el nombre actualizado
        existing.Embedding = await _aiService.GenerateEmbeddingAsync(existing.Name);
        existing.HexCode = color.HexCode;
        existing.IsActive = color.IsActive;

        await _catalogRepository.UpdateColorAsync(existing);
        return _mapper.Map<ColorDto>(existing);
    }


    // ----- CATEGORIES -----

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        Category newCategory = _mapper.Map<Category>(categoryDto);
        // Nota: Las categorías no llevan embedding, es un maestro de clasificación simple.
        await _catalogRepository.CreateCategoryAsync(newCategory);
        return _mapper.Map<CategoryDto>(newCategory);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var existing = await _catalogRepository.GetCategoryByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna categoría con el id {id}.");
        }

        await _catalogRepository.DeleteCategoryAsync(id);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(BaseQueryDto query)
    {
        IEnumerable<Category> categories = await _catalogRepository.GetAllCategoriesAsync(query);
        return categories.Select(c => _mapper.Map<CategoryDto>(c));
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        Category category = await _catalogRepository.GetCategoryByIdAsync(id);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto)
    {
        Category? existing = await _catalogRepository.GetCategoryByIdAsync(categoryDto.Id);

        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna categoría con el id {categoryDto.Id}.");
        }

        existing.Name = categoryDto.Name;
        existing.IsActive = categoryDto.IsActive;

        await _catalogRepository.UpdateCategoryAsync(existing);
        return _mapper.Map<CategoryDto>(existing);
    }

    // ----- SIZE GROUPS -----
    public async Task<IEnumerable<SizeGroupDto>> GetAllSizeGroupsAsync(BaseQueryDto query)
    {
        IEnumerable<SizeGroup> sizeGroups = await _catalogRepository.GetAllSizeGroupsAsync(query);
        return sizeGroups.Select(sg => _mapper.Map<SizeGroupDto>(sg));
    }

    public async Task<SizeGroupDto?> GetSizeGroupByIdAsync(Guid id)
    {
        SizeGroup sizeGroup = await _catalogRepository.GetSizeGroupByIdAsync(id);
        return sizeGroup != null ? _mapper.Map<SizeGroupDto>(sizeGroup) : null;
    }

    public async Task<SizeGroupDto> CreateSizeGroupAsync(SizeGroupDto sizeGroupDto)
    {
        SizeGroup newSizeGroup = _mapper.Map<SizeGroup>(sizeGroupDto);
        await _catalogRepository.CreateSizeGroupAsync(newSizeGroup);
        return _mapper.Map<SizeGroupDto>(newSizeGroup);
    }

    public async Task<SizeGroupDto> UpdateSizeGroupAsync(SizeGroupDto sizeGroupDto)
    {
        SizeGroup? existing = await _catalogRepository.GetSizeGroupByIdAsync(sizeGroupDto.Id);

        if (existing is null)
        {
            throw new NotFoundException($"No existe ningún grupo de tallas con el id {sizeGroupDto.Id}.");
        }

        existing.Name = sizeGroupDto.Name;
        existing.IsActive = sizeGroupDto.IsActive;

        await _catalogRepository.UpdateSizeGroupAsync(existing);
        return _mapper.Map<SizeGroupDto>(existing);
    }

    public async Task DeleteSizeGroupAsync(Guid id)
    {
        var existing = await _catalogRepository.GetSizeGroupByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ningún grupo de tallas con el id {id}.");
        }

        await _catalogRepository.DeleteSizeGroupAsync(id);
    }

    // ----- SIZES -----
    public async Task<IEnumerable<SizeDto>> GetAllSizesAsync(BaseQueryDto query)
    {
        IEnumerable<Size> sizes = await _catalogRepository.GetAllSizesAsync(query);
        return sizes.Select(s => _mapper.Map<SizeDto>(s));
    }

    public async Task<SizeDto?> GetSizeByIdAsync(Guid id)
    {
        Size size = await _catalogRepository.GetSizeByIdAsync(id);
        return size != null ? _mapper.Map<SizeDto>(size) : null;
    }

    public async Task<SizeDto> CreateSizeAsync(SizeDto sizeDto)
    {
        Size newSize = _mapper.Map<Size>(sizeDto);
        await _catalogRepository.CreateSizeAsync(newSize);
        return _mapper.Map<SizeDto>(newSize);
    }

    public async Task<SizeDto> UpdateSizeAsync(SizeDto sizeDto)
    {
        Size? existing = await _catalogRepository.GetSizeByIdAsync(sizeDto.Id);

        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna talla con el id {sizeDto.Id}.");
        }

        existing.Name = sizeDto.Name;
        existing.SizeGroupId = sizeDto.SizeGroupId;
        existing.IsActive = sizeDto.IsActive;

        await _catalogRepository.UpdateSizeAsync(existing);
        return _mapper.Map<SizeDto>(existing);
    }

    public async Task DeleteSizeAsync(Guid id)
    {
        var existing = await _catalogRepository.GetSizeByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna talla con el id {id}.");
        }

        await _catalogRepository.DeleteSizeAsync(id);
    }
}
