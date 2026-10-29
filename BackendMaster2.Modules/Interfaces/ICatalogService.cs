using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface ICatalogService
{
    //colores
    Task<IEnumerable<ColorDto>> GetAllColorsAsync(BaseQueryDto query);
    Task<ColorDto?> GetColorByIdAsync(Guid id);
    Task<ColorDto> CreateColorAsync(ColorDto color);
    Task<ColorDto?> UpdateColorAsync(ColorDto color);
    Task DeleteColorAsync(Guid id);
    

    //categorías
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(BaseQueryDto query);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryDto?> UpdateCategoryAsync(CategoryDto categoryDto);
    Task DeleteCategoryAsync(Guid id);

    // ----- SIZE GROUPS -----
    Task<IEnumerable<SizeGroupDto>> GetAllSizeGroupsAsync(BaseQueryDto query);
    Task<SizeGroupDto?> GetSizeGroupByIdAsync(Guid id);
    Task<SizeGroupDto> CreateSizeGroupAsync(SizeGroupDto sizeGroupDto);
    Task<SizeGroupDto> UpdateSizeGroupAsync(SizeGroupDto sizeGroupDto);
    Task DeleteSizeGroupAsync(Guid id);

    // ----- SIZES -----
    Task<IEnumerable<SizeDto>> GetAllSizesAsync(BaseQueryDto query);
    Task<SizeDto?> GetSizeByIdAsync(Guid id);
    Task<SizeDto> CreateSizeAsync(SizeDto sizeDto);
    Task<SizeDto> UpdateSizeAsync(SizeDto sizeDto);
    Task DeleteSizeAsync(Guid id);
}
