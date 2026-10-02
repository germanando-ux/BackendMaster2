using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Modules.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Services;


public class AiService : IAiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly QwenSettings _settings;

    public Task<string> EnhanceProductDescriptionAsync(EnhanceProductDescriptionRequestDto enhanceDescriptionRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
