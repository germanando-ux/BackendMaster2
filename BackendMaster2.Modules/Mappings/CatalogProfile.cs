using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackendMaster2.Modules.Mappings;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        // 1(Mapeo directo, los nombres coinciden)
        CreateMap<Color, ColorDto>().ReverseMap();
        CreateMap<Marketplace, MarketplaceDto>().ReverseMap();
        CreateMap<DataSource, DataSourceDto>().ReverseMap();
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
        CreateMap<SizeGroupDto, SizeGroup>().ReverseMap(); ;
        CreateMap<SizeDto, Size>().ReverseMap();        
        CreateMap<ProductSize, ProductSizeDto>().ReverseMap();        
        CreateMap<ProductDataSource, ProductDataSourceDto>().ReverseMap();        
        CreateMap<ProductMarketplace, ProductMarketplaceDto>().ReverseMap();



        // 2. Product -> ProductDto 
        // Necesitamos decirle explícitamente cómo sacar los ColorIds de la tabla puente.
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.ColorIds,
                       opt => opt.MapFrom(src => src.ProductOptions.Select(po => po.ColorId)));

        // 3. ProductDto -> Product
        // Ignoramos ProductOptions/ColorIds aquí porque los gestionaremos a mano 
        // en el Service para controlar los borrados/altas en la tabla puente.
        CreateMap<ProductDto, Product>().ForMember(dest => dest.ProductOptions, opt => opt.Ignore());

        // 4. Product -> ProductDetailDto (Composición para el formulario de edición)
        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.ProductOptions.Select(po => po.Color)));


        CreateMap<Product, ProductDetailDto>()
          .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src))
          .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.ProductOptions.Select(po => po.Color)))
          .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src => src.ProductSizes.Select(ps => ps.Size)))
          .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes))
          .ForMember(dest => dest.ProductDataSources, opt => opt.MapFrom(src => src.ProductDataSources))
          .ForMember(dest => dest.ProductMarketplaces, opt => opt.MapFrom(src => src.ProductMarketplaces));
    }
}
}
