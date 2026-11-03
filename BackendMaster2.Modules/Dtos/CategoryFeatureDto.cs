using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class CategoryFeatureDto
{
    public Guid FeatureId { get; set; }
    public bool IsRequired { get; set; }
}