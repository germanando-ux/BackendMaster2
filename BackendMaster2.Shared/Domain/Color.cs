using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain;

public class Color
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; }

}