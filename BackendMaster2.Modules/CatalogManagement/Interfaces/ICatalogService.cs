using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.CatalogManagement.Interfaces;

public interface ICatalogService
{
    Task<ColorDto> CreateColorAsync(ColorDto color);
    Task<IEnumerable<ColorDto>> GetAllColorsAsync();
    Task<ColorDto?> GetColorByIdAsync(Guid id);
    Task<ColorDto?> UpdateColorAsync(ColorDto color);
}
