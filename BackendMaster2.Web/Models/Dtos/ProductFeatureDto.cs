using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public class ProductFeatureDto
{
    public Guid FeatureId { get; set; }
    public string Value { get; set; } = string.Empty;
    public Guid? ChannelId { get; set; } // null = global, Guid = específico de canal
}