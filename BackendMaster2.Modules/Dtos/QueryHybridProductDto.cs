using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Pgvector;

namespace BackendMaster2.Modules.Dtos;


/// <summary>
/// DTO para la búsqueda inteligente por cosenos de vectores con los campos no discretos ya convertidos en vectores
/// </summary>
public class QueryHybridProductDto
{
    public Vector? ProductVector { get; set; }
    public Vector? ColorVector { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinStock { get; set; }
    public List<string> SizeNames { get; set; } = new List<string>();
    public int? topK { get; set; } = 10;
}
