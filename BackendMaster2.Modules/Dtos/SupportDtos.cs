using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Dtos;

/// <summary>
/// Clase cajón de sastre para DTOs ad-hoc
/// </summary>
/// 

//DTO para la búsqueda híbrida de productos con valores determinantes y con vectores embedded
//para no pervertir la capa de dominio con un valor que no es parte del mismo.
public record ProductSearchHybridResult(Product Product, double SimilarityScore);

// DTO de respuesta para el endpoint de búsqueda híbrida. Misma jugada que en ProductSearchHybridResult
// pero con el ProductDto para no pervertir el dto con un valor de similitud que no es parte del dominio.
public record ProductSearchHybridDto(ProductDto Product, double SimilarityScore);
