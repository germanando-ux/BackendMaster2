using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackendMaster2.Modules.Mappings;

public class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        // 1. Color <-> ColorDto (Mapeo directo, los nombres coinciden)
        CreateMap<Color, ColorDto>().ReverseMap();

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
    }
}
