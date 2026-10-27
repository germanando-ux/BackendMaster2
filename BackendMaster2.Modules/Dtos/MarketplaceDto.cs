using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class MarketplaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
