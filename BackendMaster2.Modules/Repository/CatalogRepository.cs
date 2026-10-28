using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;
using BackendMaster2.Shared.Extensions;

namespace BackendMaster2.Modules.Repository;

public class CatalogRepository : ICatalogRepository
{
    private readonly AppDbContext _context;

    //obtenemos la lista de propiedades públicas de la clase Color que son de tipo simple (string, int, etc.)
    //y las guardamos en un HashSet para validación rápida, es estática y se calcula una sola vez al inicio de la aplicación
    private static readonly HashSet<string> AllowedSortColumns = typeof(Color)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => TypeValidationExtensions.IsSimpleType(p.PropertyType))
        .Select(p => p.Name)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    // HashSet para validación segura de Dynamic LINQ en Categorías
    private static readonly HashSet<string> AllowedCategorySortColumns = typeof(Category)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => TypeValidationExtensions.IsSimpleType(p.PropertyType))
        .Select(p => p.Name)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public CatalogRepository(AppDbContext context)
    {
        _context = context;
    }

    //colores

    public async Task CreateColorAsync(Color color)
    {
        await _context.Color.AddAsync(color);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteColorAsync(Guid id)
    {
        var color = await GetColorByIdAsync(id);
        if (color != null)
        {
            _context.Color.Remove(color);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Color>> GetAllColorsAsync(BaseQueryDto query)
    {
        IQueryable<Color> colorsQuery = _context.Color.AsQueryable();

        //Si tiene para filtrar y está entre las columnas de la tabla
        if (!string.IsNullOrWhiteSpace(query.SearchTerm) && !string.IsNullOrWhiteSpace(query.SearchBy) && AllowedSortColumns.Contains(query.SearchBy))
        {
            colorsQuery = colorsQuery.Where($"{query.SearchBy}.ToLower().Contains(@0)", query.SearchTerm.ToLower());
        }
        if (query.IsActive.HasValue)
        {
            colorsQuery = colorsQuery.Where(p => p.IsActive == query.IsActive.Value);
        }
        if (!string.IsNullOrEmpty(query.SortBy) && AllowedSortColumns.Contains(query.SortBy))
        {
            // Si pasa el filtro, es 100% seguro usar Dynamic.Core
            string direction = query.SortAscending ? "ascending" : "descending";
            colorsQuery = colorsQuery.OrderBy($"{query.SortBy} {direction}");
        }
        else
        {
            // Si viene vacío, o intentaron inyectar una propiedad de navegación/no existente
            colorsQuery = colorsQuery.OrderBy(p => p.Name); // Orden por defecto
        }

        return await colorsQuery.ToListAsync();
    }

    public async Task<Color?> GetColorByIdAsync(Guid id)
    {
        return await _context.Color.FindAsync(id) ?? throw new NotFoundException($"Color no encontrado");
    }

    public async Task<Color> UpdateColorAsync(Color color)
    {
        _context.Color.Update(color);
        await _context.SaveChangesAsync();
        return color;
    }

    //categorías
    public async Task CreateCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await GetCategoryByIdAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(BaseQueryDto query)
    {
        IQueryable<Category> categoriesQuery = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm) && !string.IsNullOrWhiteSpace(query.SearchBy) && AllowedCategorySortColumns.Contains(query.SearchBy))
        {
            categoriesQuery = categoriesQuery.Where($"{query.SearchBy}.ToLower().Contains(@0)", query.SearchTerm.ToLower());
        }

        if (query.IsActive.HasValue)
        {
            categoriesQuery = categoriesQuery.Where(c => c.IsActive == query.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(query.SortBy) && AllowedCategorySortColumns.Contains(query.SortBy))
        {
            string direction = query.SortAscending ? "ascending" : "descending";
            categoriesQuery = categoriesQuery.OrderBy($"{query.SortBy} {direction}");
        }
        else
        {
            categoriesQuery = categoriesQuery.OrderBy(c => c.Name);
        }

        return await categoriesQuery.ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }


}
