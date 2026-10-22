using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Pgvector;

namespace BackendMaster2.Modules.Dtos;

public class QueryHybridProductDto
{
    public Vector? ProductVector { get; set; }
    public Vector? ColorVector { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinStock { get; set; }
    public int? topK { get; set; } = 10;
}
