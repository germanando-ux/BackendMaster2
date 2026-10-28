using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface ICatalogRepository
{
    //colores
    Task CreateColorAsync(Color newColor);
    Task DeleteColorAsync(Guid id);
    Task<IEnumerable<Color>> GetAllColorsAsync(BaseQueryDto query);
    Task<Color> GetColorByIdAsync(Guid id);
    Task<Color> UpdateColorAsync(Color existing);

    //categorías
    Task<IEnumerable<Category>> GetAllCategoriesAsync(BaseQueryDto query);
    Task<Category?> GetCategoryByIdAsync(Guid id);
    Task CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid id);
}
