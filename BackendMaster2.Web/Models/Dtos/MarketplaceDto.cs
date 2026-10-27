using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public class MarketplaceDto
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "El nombre del marketplace es obligatorio.")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "El código del marketplace es obligatorio.")]
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
