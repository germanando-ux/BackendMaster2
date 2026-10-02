using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.CatalogManagement.Dtos;

public class EnhanceProductDescriptionRequestDto
{
    public string TaskId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new(); 
    public string CurrentDescription { get; set; } = string.Empty;
}