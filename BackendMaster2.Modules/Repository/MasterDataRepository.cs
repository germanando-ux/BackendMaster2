using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using BackendMaster2.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace BackendMaster2.Modules.Repository;




public class MasterDataRepository : IMasterDataRepository
{
    private readonly AppDbContext _context;

    //obtenemos la lista de propiedades públicas de la clase Marketplace que son de tipo simple (string, int, etc.)
    //y las guardamos en un HashSet para validación rápida, es estática y se calcula una sola vez al inicio de la aplicación
    private static readonly HashSet<string> AllowedSortMarketplaceColumns = typeof(Marketplace)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => TypeValidationExtensions.IsSimpleType(p.PropertyType))
        .Select(p => p.Name)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    //obtenemos la lista de propiedades públicas de la clase Marketplace que son de tipo simple (string, int, etc.)
    //y las guardamos en un HashSet para validación rápida, es estática y se calcula una sola vez al inicio de la aplicación
    private static readonly HashSet<string> AllowedSortDataSourceColumns = typeof(DataSource)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => TypeValidationExtensions.IsSimpleType(p.PropertyType))
        .Select(p => p.Name)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);


    public MasterDataRepository(AppDbContext context)
    {
        _context = context;
    }

    //MARKETPLACES METHODS
    public async Task<IEnumerable<Marketplace>> GetAllMarketplacesAsync(BaseQueryDto query)
    {
        IQueryable<Marketplace> marketplacesQuery = _context.Marketplaces.AsQueryable();

        //Si tiene para filtrar y está entre las columnas de la tabla
        if (!string.IsNullOrWhiteSpace(query.SearchTerm) && !string.IsNullOrWhiteSpace(query.SearchBy) && AllowedSortMarketplaceColumns.Contains(query.SearchBy))
        {
            marketplacesQuery = marketplacesQuery.Where($"{query.SearchBy}.ToLower().Contains(@0)", query.SearchTerm.ToLower());
        }
        if (query.IsActive.HasValue)
        {
            marketplacesQuery = marketplacesQuery.Where(p => p.IsActive == query.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(query.SortBy) && AllowedSortMarketplaceColumns.Contains(query.SortBy))
        {
            // Si pasa el filtro, es 100% seguro usar Dynamic.Core
            string direction = query.SortAscending ? "ascending" : "descending";
            marketplacesQuery = marketplacesQuery.OrderBy($"{query.SortBy} {direction}");
        }
        else
        {
            // Si viene vacío, o intentaron inyectar una propiedad de navegación/no existente
            marketplacesQuery = marketplacesQuery.OrderBy(p => p.Name); // Orden por defecto
        }
        return marketplacesQuery.ToList();
    }

    public async Task<Marketplace> GetMarketplaceByIdAsync(Guid id)
    {
        return await _context.Marketplaces.FindAsync(id) ?? throw new NotFoundException($"Marketplace no encontrado");  ;   
    }

    public async Task AddMarketplaceAsync(Marketplace newMarketplace)
    {
        await _context.Marketplaces.AddAsync(newMarketplace);
        await _context.SaveChangesAsync();
    }

    public async Task<Marketplace> UpdateMarketplaceAsync(Marketplace marketplace)
    {
        _context.Marketplaces.Update(marketplace);
        await _context.SaveChangesAsync();
        return marketplace;
    }

    public async Task DeleteMarketplaceAsync(Guid id)
    {
        var marketplace = await GetMarketplaceByIdAsync(id);
        if (marketplace != null)
        {
            _context.Marketplaces.Remove(marketplace);
            await _context.SaveChangesAsync();
        }
    }
    //DATASOURCE METHODS

    public async Task<IEnumerable<DataSource>> GetAllDataSourcesAsync(BaseQueryDto query)
    {
        IQueryable<DataSource> dataSourceQuery = _context.DataSources.AsQueryable();

        //Si tiene para filtrar y está entre las columnas de la tabla
        if (!string.IsNullOrWhiteSpace(query.SearchTerm) && !string.IsNullOrWhiteSpace(query.SearchBy) && AllowedSortDataSourceColumns.Contains(query.SearchBy))
        {
            dataSourceQuery = dataSourceQuery.Where($"{query.SearchBy}.ToLower().Contains(@0)", query.SearchTerm.ToLower());
        }
        if (query.IsActive.HasValue)
        {
            dataSourceQuery = dataSourceQuery.Where(p => p.IsActive == query.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(query.SortBy) && AllowedSortDataSourceColumns.Contains(query.SortBy))
        {
            // Si pasa el filtro, es 100% seguro usar Dynamic.Core
            string direction = query.SortAscending ? "ascending" : "descending";
            dataSourceQuery = dataSourceQuery.OrderBy($"{query.SortBy} {direction}");
        }
        else
        {
            // Si viene vacío, o intentaron inyectar una propiedad de navegación/no existente
            dataSourceQuery = dataSourceQuery.OrderBy(p => p.Name); // Orden por defecto
        }
        return dataSourceQuery.ToList();
    }
    public async Task<DataSource> GetDataSourceByIdAsync(Guid id)
    {
        return await _context.DataSources.FindAsync(id) ?? throw new NotFoundException($"DataSource no encontrado"); ;
    }


    public async Task AddDataSourceAsync(DataSource newDataSource)
    {
        await _context.DataSources.AddAsync(newDataSource);
        await _context.SaveChangesAsync();
    }
    public async Task<DataSource> UpdateDataSourceAsync(DataSource datasource)
    {
        _context.DataSources.Update(datasource);
        await _context.SaveChangesAsync();
        return datasource;
    }

    public async Task DeleteDataSourceAsync(Guid id)
    {
        var datasource = await GetDataSourceByIdAsync(id);
        if (datasource != null)
        {
            _context.DataSources.Remove(datasource);
            await _context.SaveChangesAsync();
        }
    }


}
