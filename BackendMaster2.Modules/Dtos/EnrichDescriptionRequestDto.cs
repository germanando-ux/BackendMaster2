using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class EnrichDescriptionRequestDto
{
    public string TaskId { get; set; } = string.Empty;
    public string CurrentDescription { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new();
    public decimal Price { get; set; }
    public string SRConnectionId { get; set; } = string.Empty; 
}