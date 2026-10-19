using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class BaseQueryDto
{
    public string? SortBy { get; set; } = "Name";
    public bool SortAscending { get; set; } = true;
    public string? SearchBy { get; set; } = "Name";
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}
