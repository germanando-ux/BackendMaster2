using BackendMaster2.Modules.CatalogManagement.Interfaces;
using BackendMaster2.Modules.Data;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackendMaster2.Modules.CatalogManagement.Repository;

public class CatalogRepository : ICatalogRepository
{
    private readonly AppDbContext _context;

    public CatalogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddColorAsync(Color color)
    {
        _context.Color.Add(color);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Color>> GetAllColorsAsync()
    {
        return await _context.Color.ToListAsync();
    }

    public async Task<Color?> GetColorByIdAsync(Guid id)
    {
        return await _context.Color.FindAsync(id) ?? throw new NotFoundException($"Color no encontrado");
    }

    public async Task<Color> UpdateColorAsync(Color color)
    {
        _context.Color.Update(color);
        await _context.SaveChangesAsync();
        return color;
    }
}
