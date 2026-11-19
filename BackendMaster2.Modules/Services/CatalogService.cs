using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Modules.Repository;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BackendMaster2.Modules.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IMapper _mapper;
    private readonly IAiService _aiService;
    private readonly ICacheService _cacheService;


    public CatalogService( ICatalogRepository CatalogRepository, IMapper mapper, IAiService aiService, ICacheService cacheService)
    {
        _catalogRepository = CatalogRepository;
        _mapper = mapper;
        _aiService = aiService;
        _cacheService = cacheService;
    }


    //colores
    public async Task<ColorDto> CreateColorAsync(ColorDto colorDto)
    {
        Color newColor = _mapper.Map<Color>(colorDto);
        //genero vector embedding para el nombre del color
        newColor.Embedding = await _aiService.GenerateEmbeddingAsync(newColor.Name);
        await _catalogRepository.CreateColorAsync(newColor);


        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:colors:actives");
        await _cacheService.RemoveAsync("maestros:colors:all");

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:colors:actives");
        await _cacheService.RemoveAsync("maestros:colors:all");
    }

    public async Task<IEnumerable<ColorDto>> GetAllColorsAsync(BaseQueryDto query)
    {
        //detectamos si la consulta es "base" (sin searchTerm y con isActive true o null) para poder cachearla. Si no es base, no la cacheamos.
        string cachekey = $"maestros:colors:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);
        if (isBaseQuery)
        {
            List<ColorDto> cachedColors = await _cacheService.GetCacheAsync<List<ColorDto>>(cachekey);
            if (cachedColors != null)
            {
                return cachedColors;
            }
        }

        IEnumerable<Color> colors = await _catalogRepository.GetAllColorsAsync(query);
        List<ColorDto> colorsDto = colors.Select(c => _mapper.Map<ColorDto>(c)).ToList();


        if (isBaseQuery)
        { 
            await  _cacheService.SetCacheAsync(cachekey, colorsDto);
        }
        return colorsDto;
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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:colors:actives");
        await _cacheService.RemoveAsync("maestros:colors:all");

        return _mapper.Map<ColorDto>(existing);
    }


    // ----- CATEGORIES -----

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        Category newCategory = _mapper.Map<Category>(categoryDto);
        // Nota: Las categorías no llevan embedding, es un maestro de clasificación simple.

        //añadimos las características . EF Core se encarga de los DELETE/INSERT.
        newCategory.CategoryFeatures = categoryDto.CategoryFeatures.Select(cfDto => new CategoryFeature
        {
            CategoryId = newCategory.Id,
            FeatureId = cfDto.FeatureId,
            IsRequired = cfDto.IsRequired
        }).ToList();

        await _catalogRepository.CreateCategoryAsync(newCategory);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:categories:actives");
        await _cacheService.RemoveAsync("maestros:categories:all");

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:categories:actives");
        await _cacheService.RemoveAsync("maestros:categories:all");
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(BaseQueryDto query)
    {

        //detectamos si la consulta es "base" (sin searchTerm y con isActive true o null) para poder cachearla. Si no es base, no la cacheamos.
        string cachekey = $"maestros:categories:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);
        if (isBaseQuery)
        {
            List<CategoryDto> cachedCategories = await _cacheService.GetCacheAsync<List<CategoryDto>>(cachekey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }
        }

        IEnumerable<Category> categories = await _catalogRepository.GetAllCategoriesAsync(query);
        List<CategoryDto> categoriesDto = categories.Select(c => _mapper.Map<CategoryDto>(c)).ToList();


        if (isBaseQuery)
        {
            await _cacheService.SetCacheAsync<List<CategoryDto>>(cachekey, categoriesDto);
        }
        return categoriesDto;

       
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

        //borramos las características actuales y añadimos las nuevas. EF Core se encarga de los DELETE/INSERT.
        //en este caso no es una lista de ids, sino un DTO completo con datasourceId, externalCode y lastSyncedAt
        existing.CategoryFeatures.Clear();
        foreach (var cfDto in categoryDto.CategoryFeatures)
        {
            existing.CategoryFeatures.Add(new CategoryFeature            
            {
                CategoryId = existing.Id,
                FeatureId = cfDto.FeatureId,
                IsRequired = cfDto.IsRequired
            });
        }


        await _catalogRepository.UpdateCategoryAsync(existing);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:categories:actives");
        await _cacheService.RemoveAsync("maestros:categories:all");

        return _mapper.Map<CategoryDto>(existing);
    }


    public async Task<IEnumerable<CategoryFeaturesDto>> GetMandatoryFeaturesAsync(Guid id)
    {
        return await _catalogRepository.GetMandatoryFeatures(id);
    }

    // ----- SIZE GROUPS -----
    public async Task<IEnumerable<SizeGroupDto>> GetAllSizeGroupsAsync(BaseQueryDto query)
    {
        //detectamos si la consulta es "base" (sin searchTerm y con isActive true o null) para poder cachearla. Si no es base, no la cacheamos.
        string cachekey = $"maestros:sizegroups:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);
        if (isBaseQuery)
        {
            List<SizeGroupDto> cachedSizeGroups = await _cacheService.GetCacheAsync<List<SizeGroupDto>>(cachekey);
            if (cachedSizeGroups != null)
            {
                return cachedSizeGroups;
            }
        }

        IEnumerable<SizeGroup> sizeGroups = await _catalogRepository.GetAllSizeGroupsAsync(query);
        List<SizeGroupDto> sizeGroupsDto = sizeGroups.Select(sg => _mapper.Map<SizeGroupDto>(sg)).ToList();


        if (isBaseQuery)
        {
            await _cacheService.SetCacheAsync(cachekey, sizeGroupsDto);
        }
        return sizeGroupsDto;


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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:sizegroups:actives");
        await _cacheService.RemoveAsync("maestros:sizegroups:all");

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:sizegroups:actives");
        await _cacheService.RemoveAsync("maestros:sizegroups:all");

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:sizegroups:actives");
        await _cacheService.RemoveAsync("maestros:sizegroups:all");
    }

    // ----- SIZES -----
    public async Task<IEnumerable<SizeDto>> GetAllSizesAsync(BaseQueryDto query)
    {
        //detectamos si la consulta es "base" (sin searchTerm y con isActive true o null) para poder cachearla. Si no es base, no la cacheamos.
        string cachekey = $"maestros:sizes:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);
        if (isBaseQuery)
        {
            List<SizeDto> cachedSizes = await _cacheService.GetCacheAsync<List<SizeDto>>(cachekey);
            if (cachedSizes != null)
            {
                return cachedSizes;
            }
        }

        IEnumerable<Size> sizes = await _catalogRepository.GetAllSizesAsync(query);
        List<SizeDto> sizesDto = sizes.Select(s => _mapper.Map<SizeDto>(s)).ToList();


        if (isBaseQuery)
        {
            await _cacheService.SetCacheAsync(cachekey, sizesDto);
        }
        return sizesDto;

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:sizes:actives");
        await _cacheService.RemoveAsync("maestros:sizes:all");

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:sizes:actives");
        await _cacheService.RemoveAsync("maestros:sizes:all");

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

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:sizes:actives");
        await _cacheService.RemoveAsync("maestros:sizes:all");

    }


    // ----- FEATURES -----
    public async Task<IEnumerable<FeatureDto>> GetAllFeaturesAsync(BaseQueryDto query)
    {
        //detectamos si la consulta es "base" (sin searchTerm y con isActive true o null) para poder cachearla. Si no es base, no la cacheamos.
        string cachekey = $"maestros:features:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);
        if (isBaseQuery)
        {
            List<FeatureDto> cachedFeatures = await _cacheService.GetCacheAsync<List<FeatureDto>>(cachekey);
            if (cachedFeatures != null)
            {
                return cachedFeatures;
            }
        }

        IEnumerable<Feature> features = await _catalogRepository.GetAllFeaturesAsync(query);
        List<FeatureDto> featuresDto = features.Select(f => _mapper.Map<FeatureDto>(f)).ToList();


        if (isBaseQuery)
        {
            await _cacheService.SetCacheAsync(cachekey, featuresDto);
        }
        return featuresDto;

        
    }

    public async Task<FeatureDto?> GetFeatureByIdAsync(Guid id)
    {
        Feature feature = await _catalogRepository.GetFeatureByIdAsync(id);
        return feature != null ? _mapper.Map<FeatureDto>(feature) : null;
    }

    public async Task<FeatureDto> CreateFeatureAsync(FeatureDto featureDto)
    {
        Feature newFeature = _mapper.Map<Feature>(featureDto);
        await _catalogRepository.CreateFeatureAsync(newFeature);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:features:actives");
        await _cacheService.RemoveAsync("maestros:features:all");

        return _mapper.Map<FeatureDto>(newFeature);
    }

    public async Task<FeatureDto> UpdateFeatureAsync(FeatureDto featureDto)
    {
        Feature? existing = await _catalogRepository.GetFeatureByIdAsync(featureDto.Id);

        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna característica con el id {featureDto.Id}.");
        }

        existing.Name = featureDto.Name;
        existing.DataType = featureDto.DataType;
        existing.IsRequired = featureDto.IsRequired;

        //borramos las opciones actuales y añadimos las nuevas. EF Core se encarga de los DELETE/INSERT.
        existing.FeatureOptions.Clear();
        foreach (var foDto in featureDto.FeatureOptions)
        {
            existing.FeatureOptions.Add(new FeatureOption
            {                
                FeatureId = foDto.FeatureId,
                Value = foDto.Value
            });
        }

        await _catalogRepository.UpdateFeatureAsync(existing);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:features:actives");
        await _cacheService.RemoveAsync("maestros:features:all");

        return _mapper.Map<FeatureDto>(existing);
    }

    public async Task DeleteFeatureAsync(Guid id)
    {
        var existing = await _catalogRepository.GetFeatureByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna característica con el id {id}.");
        }

        await _catalogRepository.DeleteFeatureAsync(id);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:features:actives");
        await _cacheService.RemoveAsync("maestros:features:all");
    }


    // ----- FEATURES OPTIONS -----
    public async Task<IEnumerable<FeatureOptionDto>> GetAllFeatureOptionsAsync(BaseQueryDto query)
    {
        //detectamos si la consulta es "base" (sin searchTerm y con isActive true o null) para poder cachearla. Si no es base, no la cacheamos.
        string cachekey = $"maestros:featureoptions:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);
        if (isBaseQuery)
        {
            List<FeatureOptionDto> cachedFeatureOptions = await _cacheService.GetCacheAsync<List<FeatureOptionDto>>(cachekey);
            if (cachedFeatureOptions != null)
            {
                return cachedFeatureOptions;
            }
        }
            
        IEnumerable<FeatureOption> featureOptions = await _catalogRepository.GetAllFeatureOptionsAsync(query);
        List<FeatureOptionDto> featureOptionsDto = featureOptions.Select(fo => _mapper.Map<FeatureOptionDto>(fo)).ToList();


        if (isBaseQuery)
        {
            await _cacheService.SetCacheAsync(cachekey, featureOptionsDto);
        }
        return featureOptionsDto;


    }

    public async Task<FeatureOptionDto?> GetFeatureOptionByIdAsync(Guid id)
    {
        FeatureOption featureOption = await _catalogRepository.GetFeatureOptionByIdAsync(id);
        return featureOption != null ? _mapper.Map<FeatureOptionDto>(featureOption) : null;
    }

    public async Task<FeatureOptionDto> CreateFeatureOptionAsync(FeatureOptionDto featureOptionDto)
    {
        FeatureOption newFeatureOption = _mapper.Map<FeatureOption>(featureOptionDto);
        await _catalogRepository.CreateFeatureOptionAsync(newFeatureOption);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:featureoptions:actives");
        await _cacheService.RemoveAsync("maestros:featureoptions:all");

        return _mapper.Map<FeatureOptionDto>(newFeatureOption);
    }

    public async Task<FeatureOptionDto> UpdateFeatureOptionAsync(FeatureOptionDto featureOptionDto)
    {
        FeatureOption? existing = await _catalogRepository.GetFeatureOptionByIdAsync(featureOptionDto.Id);

        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna opción de característica con el id {featureOptionDto.Id}.");
        }

        existing.Value   = featureOptionDto.Value;

        await _catalogRepository.UpdateFeatureOptionAsync(existing);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:featureoptions:actives");
        await _cacheService.RemoveAsync("maestros:featureoptions:all");

        return _mapper.Map<FeatureOptionDto>(existing);
    }

    public async Task DeleteFeatureOptionAsync(Guid id)
    {
        var existing = await _catalogRepository.GetFeatureOptionByIdAsync(id);
        if (existing is null)
        {
            throw new NotFoundException($"No existe ninguna opción de característica con el id {id}.");
        }

        await _catalogRepository.DeleteFeatureOptionAsync(id);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:featureoptions:actives");
        await _cacheService.RemoveAsync("maestros:featureoptions:all");
    }

   

}
