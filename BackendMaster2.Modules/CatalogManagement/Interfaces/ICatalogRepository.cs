using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.CatalogManagement.Interfaces;

public interface ICatalogRepository
{
    Task AddColorAsync(Color newColor);
    Task<IEnumerable<Color>> GetAllColorsAsync();
    Task<Color> GetColorByIdAsync(Guid id);
    Task<Color> UpdateColorAsync(Color existing);
}
