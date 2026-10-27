using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public class DataSourceDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre del origen de datos es obligatorio.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El código del origen de datos es obligatorio.")]
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}