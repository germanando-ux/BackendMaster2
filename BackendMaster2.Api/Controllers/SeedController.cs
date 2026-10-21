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

    public SeedController(ICatalogService catalogService, IProductService productService)
    {
        _catalogService = catalogService;
        _productService = productService;
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SeedResultDto>> SeedAsync(CancellationToken cancellationToken)
    {
        var result = new SeedResultDto();
        var startTime = DateTime.UtcNow;

        try
        {
            // 1. Crear colores y guardar sus IDs
            var colorIds = new List<Guid>();
            foreach (var colorData in _seedColors)
            {
                try
                {
                    var colorDto = new ColorDto
                    {
                        Name = colorData.Name,
                        HexCode = colorData.HexCode,
                        IsActive = true
                    };

                    var created = await _catalogService.CreateColorAsync(colorDto);
                    colorIds.Add(created.Id);
                    result.ColorsCreated++;
                }
                catch (Exception)
                {
                    // Probablemente ya existe (idempotencia)
                    result.ColorsSkipped++;
                }

                await Task.Delay(1100, cancellationToken); // Rate limit Gemini: 1 RPS
            }

            // 2. Crear productos
            foreach (var productData in _seedProducts)
            {
                try
                {
                    // Asignar 2-3 colores aleatorios de los creados
                    var randomColors = colorIds
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(productData.ColorCount)
                        .ToList();

                    var productDto = new ProductDto
                    {
                        Sku = productData.Sku,
                        Name = productData.Name,
                        Description = productData.Description,
                        Price = productData.Price,
                        Stock = productData.Stock,
                        IsActive = true,
                        ColorIds = randomColors
                    };

                    await _productService.CreateAsync(productDto);
                    result.ProductsCreated++;
                }
                catch (Exception)
                {
                    // Probablemente SKU duplicado (idempotencia)
                    result.ProductsSkipped++;
                }

                await Task.Delay(1100, cancellationToken); // Rate limit Gemini: 1 RPS
            }

            result.TotalTimeSeconds = (int)(DateTime.UtcNow - startTime).TotalSeconds;
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Datos de seed
    private static readonly List<(string Name, string HexCode)> _seedColors = new()
    {
        ("Rojo Ferrari", "#D40000"),
        ("Azul Marino", "#000080"),
        ("Verde Bosque", "#228B22"),
        ("Negro Mate", "#1C1C1C"),
        ("Blanco Perla", "#F0EDE2"),
        ("Gris Plata", "#C0C0C0"),
        ("Amarillo Sol", "#FFD700"),
        ("Naranja Fuego", "#FF4500"),
        ("Rosa Palo", "#FADADD"),
        ("Morado Vino", "#722F37")
    };

    private static readonly List<(string Sku, string Name, string Description, decimal Price, int Stock, int ColorCount)> _seedProducts = new()
    {
        ("ZAP-RUN-001", "Zapatillas Running Pro", "Zapatillas de running de alto rendimiento con suela de espuma reactiva y upper transpirable de malla técnica. Diseñadas para corredores que buscan máximo confort en largas distancias.", 129.99m, 45, 3),
        ("CAM-ALG-002", "Camiseta Algodón Premium", "Camiseta de algodón orgánico 100% con corte clásico y acabado suave al tacto. Costuras reforzadas y etiqueta exterior para mayor comodidad.", 29.99m, 120, 2),
        ("PAN-CHI-003", "Pantalón Chino Slim Fit", "Pantalón chino de corte slim fabricado en sarga de algodón elástica. Perfecto para looks casuales y de oficina informal.", 59.99m, 80, 2),
        ("CHA-IMP-004", "Chaqueta Impermeable Urban", "Chaqueta urbana impermeable con costuras selladas y capucha ajustable. Tejido transpirable ideal para días de lluvia en la ciudad.", 89.99m, 35, 3),
        ("MOC-URB-005", "Mochila Urbana 20L", "Mochila de 20 litros con compartimento acolchado para portátil de hasta 15 pulgadas. Múltiples bolsillos organizadores y correas ergonómicas.", 69.99m, 60, 2),
        ("GOR-DEP-006", "Gorra Deportiva Transpirable", "Gorra deportiva con paneles de malla transpirable y cierre ajustable en la parte trasera. Protección UV y banda interior absorbente.", 19.99m, 150, 2),
        ("REL-MIN-007", "Reloj Minimalista Acero", "Reloj de pulsera con caja de acero inoxidable de 40mm y correa de malla milanesa. Movimiento de cuarzo japonés y resistencia al agua 5ATM.", 149.99m, 25, 3),
        ("GAF-SOL-008", "Gafas de Sol Polarizadas", "Gafas de sol con lentes polarizadas que eliminan reflejos y protegen 100% contra rayos UVA y UVB. Montura ligera de acetato italiano.", 79.99m, 70, 2),
        ("CIN-CUE-009", "Cinturón Cuero Genuino", "Cinturón de cuero genuino con hebilla de acero cepillado. Ancho de 3.5cm y disponible en varias tallas. Acabado artesanal.", 39.99m, 90, 2),
        ("ZAP-OXF-010", "Zapatos Oxford Clásicos", "Zapatos Oxford de piel pulida con suela de cuero cosida Goodyear. Elegancia atemporal para ocasiones formales y de negocios.", 179.99m, 30, 2),
        ("SUD-CAP-011", "Sudadera con Capucha Oversize", "Sudadera oversize de algodón orgánico con capucha ajustable y bolsillo canguro. Interior afelpado para máximo confort.", 54.99m, 100, 3),
        ("SHO-DEP-012", "Shorts Deportivos Ligeros", "Shorts deportivos de tejido técnico ultraligero con secado rápido. Cintura elástica con cordón y bolsillo interior con cremallera.", 34.99m, 110, 2),
        ("BOL-BAN-013", "Bolso Bandolera Piel", "Bolso bandolera de piel vacuno con compartimento principal y dos bolsillos exteriores. Correa ajustable y cierre magnético.", 99.99m, 40, 3),
        ("CAR-PIE-014", "Cartera Piel Compacta", "Cartera compacta de piel con 6 ranuras para tarjetas, compartimento para billetes y bolsillo para monedas con cremallera.", 49.99m, 85, 2),
        ("PAR-COM-015", "Paraguas Compacto Automático", "Paraguas compacto de apertura y cierre automático con estructura reforzada contra el viento. Incluye funda a juego.", 24.99m, 130, 2),
        ("BUF-LAN-016", "Bufanda Lana Merino", "Bufanda de lana merino extrafina, suave al tacto y muy cálida. Dimensiones 180x30cm. Hipoalergénica y termorreguladora.", 44.99m, 75, 3),
        ("GUA-TAC-017", "Guantes Táctiles Invierno", "Guantes de invierno con pantalla táctil en índice y pulgar. Interior afelpado y palma antideslizante. Ideales para usar el móvil con frío.", 29.99m, 95, 2),
        ("CAL-RUN-018", "Calcetines Running Pack x3", "Pack de 3 pares de calcetines deportivos con refuerzo en talón y puntera. Tejido transpirable con canales de ventilación.", 19.99m, 200, 2),
        ("CAM-LIN-019", "Camisa Lino Verano", "Camisa de lino 100% natural, fresca y ligera para el verano. Corte regular fit con cuello clásico y botones de nácar.", 64.99m, 65, 3),
        ("JER-CUE-020", "Jersey Cuello Alto Cashmere", "Jersey de cashmere puro con cuello alto y puños acanalados. Suavidad excepcional y aislamiento térmico natural.", 189.99m, 20, 2),
        ("ABR-LAN-021", "Abrigo Lana Largo", "Abrigo largo de mezcla de lana con corte entallado y solapas clásicas. Forro interior de poliéster y dos bolsillos laterales.", 219.99m, 15, 3),
        ("BER-CAR-022", "Bermudas Cargo Algodón", "Bermudas tipo cargo de algodón resistente con múltiples bolsillos con solapa. Cintura ajustable y corte recto.", 44.99m, 80, 2),
        ("POL-PIQ-023", "Polo Piqué Clásico", "Polo de algodón piqué con cuello camisero y tres botones. Corte clásico atemporal, perfecto para looks smart casual.", 39.99m, 140, 3),
        ("CHA-ACO-024", "Chaleco Acolchado Ligero", "Chaleco acolchado ultraligero con relleno sintético térmico. Cierre de cremallera y dos bolsillos laterales. Packable en su propio bolsillo.", 74.99m, 55, 2),
        ("BOT-MON-025", "Botas Montaña Impermeables", "Botas de montaña impermeables con membrana técnica y suela Vibram de alto agarre. Caña media con protección en puntera y talón.", 159.99m, 35, 3)
    };
}

public record SeedResultDto
{
    public int ColorsCreated { get; set; }
    public int ColorsSkipped { get; set; }
    public int ProductsCreated { get; set; }
    public int ProductsSkipped { get; set; }
    public int TotalTimeSeconds { get; set; }
}