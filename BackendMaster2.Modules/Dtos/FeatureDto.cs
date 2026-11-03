using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

public class FeatureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public FeatureDataType DataType { get; set; }
    public bool IsRequired { get; set; }

    // Solo se da la relación si datatype está informado
    public ICollection<FeatureOptionDto> Options { get; set; } = new List<FeatureOptionDto>();
}