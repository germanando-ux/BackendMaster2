using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class FeatureOptionDto
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }
    public string Value { get; set; } = string.Empty;
}