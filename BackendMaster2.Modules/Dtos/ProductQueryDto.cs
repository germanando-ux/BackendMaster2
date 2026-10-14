using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public record ProductQueryDto
{
    public string? SortBy { get; init; } = "Name";

    public bool SortAscending { get; init; } = true;
}
