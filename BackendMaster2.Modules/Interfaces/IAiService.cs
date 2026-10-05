using BackendMaster2.Modules.CatalogManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface IAiService
{
    Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
}