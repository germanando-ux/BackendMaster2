using BackendMaster2.Modules.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface ISearchService
{
    public Task<IEnumerable<ProductDto>> SearchProductsAsync(string query, CancellationToken cancellationToken = default);
}
