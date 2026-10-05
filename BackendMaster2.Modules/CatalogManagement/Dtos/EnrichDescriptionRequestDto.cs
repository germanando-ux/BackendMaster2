using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.CatalogManagement.Dtos;

public class EnrichDescriptionRequestDto
{
    public string CurrentDescription { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new();
    public decimal Price { get; set; }
    public string SRConnectionId { get; set; } = string.Empty; 
}