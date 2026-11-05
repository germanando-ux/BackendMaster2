using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ICollection<CategoryFeaturesDto> CategoryFeatures { get; set; } = new List<CategoryFeaturesDto>();
}
