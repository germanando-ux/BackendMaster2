using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly IProductService _productService;
    private readonly Random _random;

    public SeedController(ICatalogService catalogService, IProductService productService)
    {
        _catalogService = catalogService;
        _productService = productService;
        _random = new Random();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SeedResultDto>> SeedAsync(CancellationToken cancellationToken)
    {
        var result = new SeedResultDto();
        var startTime = DateTime.UtcNow;

        try
        {
            // 1. COLORES (Comentado según tu código)
            //var queryVacia = new BaseQueryDto();
            //var coloresExistentes = await _catalogService.GetAllColorsAsync(queryVacia);
            //var colorIdsDict = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            //foreach (var color in coloresExistentes) { colorIdsDict[color.Name] = color.Id; result.ColorsSkipped++; }
            //foreach (var colorData in _seedColors) {
            //    if (colorIdsDict.ContainsKey(colorData.Name)) continue;
            //    try {
            //        var colorDto = new ColorDto { Name = colorData.Name, HexCode = colorData.HexCode, IsActive = true };
            //        var created = await _catalogService.CreateColorAsync(colorDto);
            //        colorIdsDict[colorData.Name] = created.Id; result.ColorsCreated++;
            //        await Task.Delay(1100, cancellationToken);
            //    } catch (Exception ex) { Console.WriteLine($"[SEED] Error creando color {colorData.Name}: {ex.Message}"); }
            //}

            // 2. CATEGORÍAS (Comentado según tu código)
            //var categoriasSeed = new[] { "Ropa", "Calzado", "Electrónica", "Hogar", "Deportes" };
            //var categoriasExistentes = (await _catalogService.GetAllCategoriesAsync(new BaseQueryDto())).Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            //foreach (var catName in categoriasSeed) {
            //    if (categoriasExistentes.Contains(catName)) { result.CategoriesSkipped++; continue; }
            //    try {
            //        await _catalogService.CreateCategoryAsync(new CategoryDto { Name = catName, IsActive = true });
            //        result.CategoriesCreated++;
            //    } catch (Exception ex) { Console.WriteLine($"[SEED] Error creando categoría {catName}: {ex.Message}"); }
            //}

            // ========================================================================
            // 2.6 SEED DE SIZE GROUPS (ACTIVO E IDEMPOTENTE)
            // ========================================================================
            var sizeGroupsSeedData = new[] { "EU", "US", "UK", "INT", "FR" };
            var existingSizeGroups = await _catalogService.GetAllSizeGroupsAsync(new BaseQueryDto());
            var sizeGroupsDict = existingSizeGroups.ToDictionary(sg => sg.Name, sg => sg.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var sgName in sizeGroupsSeedData)
            {
                if (sizeGroupsDict.ContainsKey(sgName))
                {
                    result.SizeGroupsSkipped++;
                    continue;
                }

                try
                {
                    var created = await _catalogService.CreateSizeGroupAsync(new SizeGroupDto { Name = sgName, IsActive = true });
                    sizeGroupsDict[sgName] = created.Id;
                    result.SizeGroupsCreated++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SEED] Error creando SizeGroup {sgName}: {ex.Message}");
                }
            }

            // ========================================================================
            // 2.7 SEED DE SIZES (ACTIVO E IDEMPOTENTE)
            // ========================================================================
            var sizesByGroup = new Dictionary<string, string[]>
            {
                { "EU", new[] { "36", "38", "40", "42", "44", "46", "48" } },
                { "US", new[] { "4", "6", "8", "10", "12", "14" } },
                { "UK", new[] { "6", "8", "10", "12", "14" } },
                { "INT", new[] { "XS", "S", "M", "L", "XL", "XXL" } },
                { "FR", new[] { "34", "36", "38", "40", "42" } }
            };

            var existingSizes = await _catalogService.GetAllSizesAsync(new BaseQueryDto());
            var existingSizesKeys = existingSizes.Select(s => $"{s.SizeGroupId}_{s.Name.ToLower()}").ToHashSet();

            foreach (var kvp in sizesByGroup)
            {
                if (!sizeGroupsDict.TryGetValue(kvp.Key, out var sizeGroupId))
                {
                    Console.WriteLine($"[SEED] SizeGroup {kvp.Key} no encontrado, saltando sus tallas.");
                    continue;
                }

                foreach (var sizeName in kvp.Value)
                {
                    var uniqueKey = $"{sizeGroupId}_{sizeName.ToLower()}";
                    if (existingSizesKeys.Contains(uniqueKey))
                    {
                        result.SizesSkipped++;
                        continue;
                    }

                    try
                    {
                        await _catalogService.CreateSizeAsync(new SizeDto { Name = sizeName, SizeGroupId = sizeGroupId, IsActive = true });
                        existingSizesKeys.Add(uniqueKey);
                        result.SizesCreated++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SEED] Error creando Size {sizeName} para {kvp.Key}: {ex.Message}");
                    }
                }
            }
            // ========================================================================

            // 3. PRODUCTOS (Comentado según tu código)
            //var productosGenerados = GenerarProductosMock(200, colorIdsList); // Nota: colorIdsList estaría comentado también
            //foreach (var productData in productosGenerados) {
            //    try {
            //        var productDto = new ProductDto { Sku = productData.Sku, Name = productData.Name, Description = productData.Description, Price = productData.Price, Stock = productData.Stock, IsActive = true, ColorIds = productData.ColorIds };
            //        await _productService.CreateAsync(productDto);
            //        result.ProductsCreated++;
            //        await Task.Delay(1100, cancellationToken);
            //    } catch (Exception ex) {
            //        result.ProductsSkipped++;
            //        Console.WriteLine($"[SEED] Error creando producto {productData.Sku}: {ex.Message}");
            //    }
            //}

            result.TotalTimeSeconds = (int)(DateTime.UtcNow - startTime).TotalSeconds;
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Genera N productos mock con variaciones aleatorias basadas en plantillas
    /// </summary>
    private List<(string Sku, string Name, string Description, decimal Price, int Stock, List<Guid> ColorIds)>
        GenerarProductosMock(int cantidad, List<Guid> colorIdsDisponibles)
    {
        var productos = new List<(string, string, string, decimal, int, List<Guid>)>();

        var plantillas = new[]
        {
            new { Categoria = "Zapatillas", Nombre = "Zapatillas", Desc = "zapatillas deportivas de alto rendimiento con suela de goma antideslizante y sistema de amortiguación avanzado. Upper transpirable de malla técnica con refuerzos estratégicos.", PrecioMin = 49.99m, PrecioMax = 199.99m },
            new { Categoria = "Camiseta", Nombre = "Camiseta", Desc = "camiseta técnica de tejido transpirable con tecnología de secado rápido. Costuras planas para evitar rozaduras y tratamiento antibacteriano.", PrecioMin = 19.99m, PrecioMax = 59.99m },
            new { Categoria = "Pantalón", Nombre = "Pantalón", Desc = "pantalón deportivo de corte ergonómico con tejido elástico 4 direcciones. Cintura ajustable con cordón y bolsillos con cremallera.", PrecioMin = 34.99m, PrecioMax = 89.99m },
            new { Categoria = "Chaqueta", Nombre = "Chaqueta", Desc = "chaqueta impermeable con membrana técnica transpirable. Costuras selladas, capucha ajustable y múltiples bolsillos con cremallera.", PrecioMin = 69.99m, PrecioMax = 249.99m },
            new { Categoria = "Mochila", Nombre = "Mochila", Desc = "mochila ergonómica con sistema de ventilación trasera. Compartimento acolchado para portátil y múltiples organizadores interiores.", PrecioMin = 39.99m, PrecioMax = 129.99m },
            new { Categoria = "Gorra", Nombre = "Gorra", Desc = "gorra deportiva con paneles de malla transpirable y visera curvada. Cierre ajustable tipo velcro y banda interior absorbente.", PrecioMin = 14.99m, PrecioMax = 34.99m },
            new { Categoria = "Sudadera", Nombre = "Sudadera", Desc = "sudadera de algodón orgánico con capucha ajustable y bolsillo canguro. Interior afelpado para máximo confort térmico.", PrecioMin = 39.99m, PrecioMax = 89.99m },
            new { Categoria = "Shorts", Nombre = "Shorts", Desc = "shorts deportivos ultraligeros de tejido técnico con secado rápido. Forro interior de compresión y bolsillo trasero con cremallera.", PrecioMin = 24.99m, PrecioMax = 54.99m },
            new { Categoria = "Reloj", Nombre = "Reloj", Desc = "reloj deportivo con GPS integrado y monitor de frecuencia cardíaca. Resistente al agua hasta 50 metros y batería de larga duración.", PrecioMin = 89.99m, PrecioMax = 399.99m },
            new { Categoria = "Guantes", Nombre = "Guantes", Desc = "guantes técnicos con palma antideslizante y dedos compatibles con pantallas táctiles. Tejido transpirable y cierre de velcro ajustable.", PrecioMin = 19.99m, PrecioMax = 49.99m },
            new { Categoria = "Calcetines", Nombre = "Calcetines", Desc = "calcetines deportivos de compresión con refuerzo en talón y puntera. Tejido antibacteriano y canales de ventilación.", PrecioMin = 9.99m, PrecioMax = 29.99m },
            new { Categoria = "Chaleco", Nombre = "Chaleco", Desc = "chaleco acolchado ultraligero con relleno sintético térmico. Packable en su propio bolsillo y resistente al agua.", PrecioMin = 49.99m, PrecioMax = 119.99m }
        };

        var prefijos = new[] { "Pro", "Elite", "Ultra", "Max", "Air", "Lite", "Sport", "Evo", "Tech", "Prime", "Active", "Core" };
        var sufijos = new[] { "X", "Plus", "Air", "Flex", "Motion", "Force", "Speed", "Power", "One", "Go" };

        for (int i = 1; i <= cantidad; i++)
        {
            var plantilla = plantillas[_random.Next(plantillas.Length)];
            var prefijo = prefijos[_random.Next(prefijos.Length)];
            var sufijo = sufijos[_random.Next(sufijos.Length)];
            var nombre = $"{prefijo} {plantilla.Nombre} {sufijo} {i:D3}";
            var sku = $"{plantilla.Categoria.Substring(0, 3).ToUpper()}-{prefijo.Substring(0, 3).ToUpper()}-{i:D4}";

            var precio = Math.Round(plantilla.PrecioMin + ((decimal)_random.NextDouble() * (plantilla.PrecioMax - plantilla.PrecioMin)), 2);
            var stock = _random.Next(10, 250);

            var numColores = _random.Next(2, 4);
            var coloresAsignados = colorIdsDisponibles.OrderBy(_ => Guid.NewGuid()).Take(numColores).ToList();

            productos.Add((sku, nombre, plantilla.Desc, precio, stock, coloresAsignados));
        }

        return productos;
    }

    private static readonly List<(string Name, string HexCode)> _seedColors = new()
    {
        ("Rojo Ferrari", "#D40000"), ("Azul Marino", "#000080"), ("Verde Bosque", "#228B22"),
        ("Negro Mate", "#1C1C1C"), ("Blanco Perla", "#F0EDE2"), ("Gris Plata", "#C0C0C0"),
        ("Amarillo Sol", "#FFD700"), ("Naranja Fuego", "#FF4500"), ("Rosa Palo", "#FADADD"),
        ("Morado Vino", "#722F37"), ("Turquesa", "#40E0D0"), ("Burdeos", "#800020"),
        ("Beige Arena", "#F5F5DC"), ("Verde Aguamarina", "#7FFFD4"), ("Gris Carbón", "#36454F")
    };
}

public record SeedResultDto
{
    public int ColorsCreated { get; set; }
    public int ColorsSkipped { get; set; }
    public int CategoriesCreated { get; set; }
    public int CategoriesSkipped { get; set; }
    
    // NUEVO: Contadores para Tallas
    public int SizeGroupsCreated { get; set; }
    public int SizeGroupsSkipped { get; set; }
    public int SizesCreated { get; set; }
    public int SizesSkipped { get; set; }
    
    public int ProductsCreated { get; set; }
    public int ProductsSkipped { get; set; }
    public int TotalTimeSeconds { get; set; }
}

public static class RandomExtensions
{
    public static decimal NextDecimal(this Random random)
    {
        return (decimal)random.NextDouble();
    }
}