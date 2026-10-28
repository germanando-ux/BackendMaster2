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
}
