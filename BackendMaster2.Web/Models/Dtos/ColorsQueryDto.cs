using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public record ColorsQueryDto
{
    public string? SortBy { get; init; } = "Name";
    public bool SortAscending { get; init; } = true;
}
