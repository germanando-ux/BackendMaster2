using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.ProductManagement.Interface;

public interface ICatalogRepository
{
    Task<IEnumerable<Color>> GetAllColorsAsync();
    Task<Color> GetColorByIdAsync(Guid id);
}
