using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.CatalogManagement.Interfaces;

public interface ICatalogService
{
    Task<IEnumerable<Color>> GetAllColorsAsync();
    Task<Color> GetColorByIdAsync(Guid id);
}
