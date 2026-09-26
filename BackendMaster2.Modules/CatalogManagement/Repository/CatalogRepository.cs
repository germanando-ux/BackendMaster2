using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BackendMaster2.Modules.CatalogManagement.Repository;

public class CatalogRepository : ICatalogRepository
{
    private readonly AppDbContext _context;

    public CatalogRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Color>> GetAllColorsAsync()
    {
        return await _context.Color.ToListAsync();
    }

    public async Task<Color?> GetColorByIdAsync(Guid id)
    {
        return await _context.Color.FirstOrDefaultAsync(c => c.Id == id);
    }
}
