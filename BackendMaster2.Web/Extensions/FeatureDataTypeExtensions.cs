using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Extensions;

public static class FeatureDataTypeExtensions
{
  
    public static string GetDisplayName(this FeatureDataType dataType) => dataType switch
    {
        FeatureDataType.Text => "Texto",
        FeatureDataType.List => "Lista",
        FeatureDataType.Number => "Número",
        FeatureDataType.Boolean => "Sí/No",
        _ => "Desconocido"
    };
   
}
