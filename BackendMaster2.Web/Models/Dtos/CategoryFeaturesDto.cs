using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Web.Models.Dtos;

public class CategoryFeaturesDto
{
    public Guid FeatureId { get; set; }
    public bool IsRequired { get; set; }
    //añadimos el nombre de la característica para poder mostrar las características obligatorias de la categoría en el front
    public string FeatureName { get; set; } = string.Empty;
}