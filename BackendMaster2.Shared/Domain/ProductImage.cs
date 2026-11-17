using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Domain;

    /// <summary>
    /// Representa una imagen asociada a un producto.
    /// Diseñada para soportar almacenamiento en MinIO/S3 (StorageKey) 
    /// y futuras integraciones con Marketplaces (Source, ExternalId).
    /// </summary>
    public class ProductImage
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Clave foránea hacia el producto al que pertenece la imagen.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Ruta interna en el storage (ej: products/123/img.jpg). 
        /// Inmutable una vez subida la imagen.
        /// </summary>
        public string StorageKey { get; set; } = string.Empty;

        /// <summary>
        /// URL pública completa para que el frontend la pinte o para enviarla al Marketplace.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Texto alternativo para SEO y accesibilidad.
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Orden de visualización en el carrusel de imágenes.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indica si esta imagen es la principal del producto.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Origen de la imagen: "Manual", "Shopify", "Excel", etc.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// ID de la imagen en el sistema de origen (útil para sincronizaciones futuras).
        /// </summary>
        public string? ExternalId { get; set; }

        /// <summary>
        /// Fecha de creación del registro.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación (opcional, pero útil para EF Core)
        public Product? Product { get; set; }
    }
