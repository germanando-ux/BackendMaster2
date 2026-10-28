📖 Diario de Arquitectura - BackendMaster2
Entrada #1 | Fecha: 28/10/2026
🎯 Objetivo de la jornada
Implementar el backend completo para Categorías y definir/aplicar la arquitectura de base de datos para Tallas. Resolver incidencias de sincronización con Entity Framework Core sin perder datos.
🏛️ Decisiones Arquitectónicas Clave
Separación de Módulos:
Catalog: Agrupa maestros de clasificación de producto (Colores, Categorías, Tallas, Atributos).
MasterData: Agrupa orígenes y canales de distribución (DataSources, Marketplaces).
Estructura de Controladores:
Un controlador por recurso (ej. CategoryController) para mantener endpoints limpios y específicos.
El Servicio y Repositorio (CatalogService, CatalogRepository) agrupan la lógica de dominio de todos los maestros de catálogo.
Entidades y Relaciones:
ProductOption: Se mantiene exclusivamente para Colores. No se unifica con Categorías ni Tallas para evitar tablas genéricas con campos nulos y pérdida de integridad referencial.
Tallas (Sizes): Se decide desacoplar SizeGroup de Category. Un producto selecciona su Categoría y su SizeGroup de forma independiente. Esto evita acoplamientos innecesarios y over-engineering.
ProductSize: Tabla de unión N:M con payload (Stock, Sku).
Product.Stock: Se mantiene como campo denormalizado (stock total). Su actualización se hará desde la capa de servicio en C#, nunca con Triggers de BBDD.
DTOs y Validación:
Los DTOs en Modules son POCOs limpios (sin DataAnnotations).
La validación en Backend se gestiona con FluentValidation.
Los DataAnnotations se usarán exclusivamente en los DTOs del proyecto Web (Frontend Blazor).
Seed de Datos:
Debe ser idempotente (comprobar si existe antes de crear, usar continue si ya está).
Integrado en el SeedController existente, respetando las capas (llama al Servicio, no al DbContext directamente).
💡 Lecciones Aprendidas / Problemas Resueltos
Desincronización de EF Core: Borrar archivos de migración manualmente desde el IDE desincroniza el historial. EF Core intenta recrear todas las tablas al generar la siguiente migración.
Solución profesional: Nunca tumbar la base de datos en desarrollo por esto. La solución correcta es editar manualmente el archivo .cs de la nueva migración para que contenga únicamente los scripts CreateTable de las entidades que faltan realmente.
AutoMapper: No mapea automáticamente aunque las propiedades se llamen igual. Requiere CreateMap<Origen, Destino>() explícito en el Profile correspondiente (ej. CatalogProfile).
🛑 Estado Actual (Stop Point)
Categorías: Backend 100% completado (Entidades, DbContext, DTOs, Repo, Servicio, Controller, Seed).
Tallas: Entidades (SizeGroup, Size, ProductSize) creadas. AppDbContext configurado. Tablas físicas creadas correctamente en PostgreSQL (tras corregir el script de migración manualmente).
Pendiente: DTOs, Repositorio, Servicio y Controller de Tallas.
🚀 Plan para la próxima sesión
Crear los DTOs de Tallas (SizeGroupDto, SizeDto, ProductSizeDto).
Implementar Repositorio y Servicio de Tallas (ampliando ICatalogRepository / ICatalogService).
Crear SizeController (o SizeGroupController).
Integrar el Seed de Tallas en el SeedController.
Iniciar diseño de ProductSource y ProductChannel para la integración con el frontend de productos.