using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;

namespace BackendMaster2.Modules.Mappings;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        // 1. Mapeo directo (los nombres de las propiedades coinciden)
        CreateMap<Color, ColorDto>().ReverseMap();
        CreateMap<Marketplace, MarketplaceDto>().ReverseMap();
        CreateMap<DataSource, DataSourceDto>().ReverseMap();

        // Categoría: Separamos lectura y escritura por el manejo manual de CategoryFeatures
        CreateMap<Category, CategoryDto>(); // Lectura: mapea CategoryFeatures automáticamente
        CreateMap<CategoryDto, Category>()  // Escritura: ignoramos la colección
            .ForMember(dest => dest.CategoryFeatures, opt => opt.Ignore());

        CreateMap<SizeGroup, SizeGroupDto>().ReverseMap();
        CreateMap<Size, SizeDto>().ReverseMap();
        CreateMap<ProductSize, ProductSizeDto>().ReverseMap();
        CreateMap<ProductDataSource, ProductDataSourceDto>().ReverseMap();
        CreateMap<ProductMarketplace, ProductMarketplaceDto>().ReverseMap();

        // Nuevos mapeos para el sistema de Atributos (Features)
        CreateMap<Feature, FeatureDto>().ReverseMap();
        CreateMap<FeatureOption, FeatureOptionDto>().ReverseMap();
        CreateMap<ProductFeature, ProductFeatureDto>().ReverseMap();
        CreateMap<CategoryFeature, CategoryFeatureDto>().ReverseMap();

        // 2. Product -> ProductDto (Lectura para listados/edición)
        // Mapeamos explícitamente las listas de IDs desde las tablas puente
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.ColorIds,
                       opt => opt.MapFrom(src => src.ProductOptions.Select(po => po.ColorId)))
            .ForMember(dest => dest.CategoryIds,
                       opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.CategoryId)));

        // 3. ProductDto -> Product (Escritura para Create/Update)
        // Ignoramos TODAS las colecciones porque las gestionamos a mano en el ProductService 
        // para aplicar el patrón quirúrgico (Clear + Add) y evitar conflictos con el Change Tracker de EF Core.
        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.ProductOptions, opt => opt.Ignore())
            .ForMember(dest => dest.ProductCategories, opt => opt.Ignore())
            .ForMember(dest => dest.ProductSizes, opt => opt.Ignore())
            .ForMember(dest => dest.ProductDataSources, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMarketplaces, opt => opt.Ignore())
            .ForMember(dest => dest.ProductFeatures, opt => opt.Ignore());

        // 4. Product -> ProductDetailDto (Lectura profunda para el detalle)
        // AutoMapper mapea automáticamente Id, Sku, Name, Description, Price, Stock, IsActive por convención de nombres.
        // Solo necesitamos decirle explícitamente cómo mapear las colecciones anidadas.
        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.Colors,
                       opt => opt.MapFrom(src => src.ProductOptions.Select(po => po.Color)))
            .ForMember(dest => dest.Categories,
                       opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.Category)))
            .ForMember(dest => dest.ProductSizes,
                       opt => opt.MapFrom(src => src.ProductSizes))
            .ForMember(dest => dest.ProductDataSources,
                       opt => opt.MapFrom(src => src.ProductDataSources))
            .ForMember(dest => dest.ProductMarketplaces,
                       opt => opt.MapFrom(src => src.ProductMarketplaces))
            .ForMember(dest => dest.ProductFeatures,
                       opt => opt.MapFrom(src => src.ProductFeatures));
    }
}