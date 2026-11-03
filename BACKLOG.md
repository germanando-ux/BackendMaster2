# BACKLOG - BackendMaster2

## Pendientes técnicos detectados
- [ ] Paginación real de servidor en la API (skip/take) cuando los datos crezcan
- [x] Creación de DTOs específicos para respuestas complejas (ej. `ProductSearchHybridDto`) evitando ensuciar entidades de dominio con datos ad-hoc (¡Hecho!)
- [ ] Limpieza C10: borrar `NavMenu.razor` y su css huérfanos, css viejos de la plantilla, splash de arranque del index.html
- [ ] Alta conjunta User+Person desde el front (formulario del perforador)
- [x] Validación de cliente con anotaciones + pintado de ProblemDetails del middleware en el front (¡Hecho!)

## Enriquecimiento del dominio PIM

### ✅ Completado (Backend)
- [x] **Maestros base**: Entidades `DataSource` y `Marketplace` con CRUD completo (Backend), validaciones y seeds iniciales.
- [x] **Categorías**: 
  - [x] Entidad `Category` con `ParentId` (jerarquía autorreferencial).
  - [x] Tabla intermedia `ProductCategory` (relación N:M).
  - [x] Seed realista: "Ropa > Camisetas", "Calzado > Running", etc.
  - [x] Entidad `Product` relacionada con `ProductCategory`.
  - [x] Integración en `ProductService` (Create/Update/Get) con patrón quirúrgico.
- [x] **Tallas**:
  - [x] Maestros: `SizeGroup` (ej. "EU", "US") y `Size` (ej. "42", "M").
  - [x] Entidad de unión: `ProductSize` con payload (`Stock`, `Sku`, `Price`).
  - [x] Integración en `ProductService` (Create/Update/Get) con patrón quirúrgico.
  - [x] Campo `Price` (nullable) en `ProductSize` para variantes comerciales.
- [x] **Integración de relaciones N:M en Product** (sesión 30/10/2026):
  - [x] `ProductDataSource` y `ProductMarketplace` integrados en `ProductService` (Create/Update/Get).
  - [x] Patrón quirúrgico (`Clear()` + `Add()`) aplicado a las 5 relaciones de Product (Colores, Categorías, Tallas, DataSources, Marketplaces).
  - [x] `GetByIdAsync` ampliado con `ThenInclude` para cargar datos relacionados (Marketplace, DataSource, SizeGroup).
  - [x] DTOs de unión creados: `ProductDataSourceDto`, `ProductMarketplaceDto`, `ProductSizeDto`.
  - [x] AutoMapper (`CatalogProfile`) actualizado con mapeos de nuevas entidades.
  - [x] Decisión arquitectónica: **Pragmatismo sobre sobreingeniería** (no replicar complejidad de B2X/Shein; modelo simple escalable).
- [x] **Atributos Flexibles (Features)** (sesión 02/11/2026):
  - [x] Diseño: Modelo EAV relacional (descartado JSONB).
  - [x] Entidades: `Feature`, `FeatureOption`, `ProductFeature`, `CategoryFeature`.
  - [x] Migración `AddFeatures` aplicada.
  - [x] DTOs, AutoMapper, Repositorio, Servicio, Controladores y Validadores.
  - [x] Soporte para atributos específicos de canal (`ChannelId` nullable en `ProductFeature`).
  - [x] Patrón quirúrgico extendido a `CategoryFeatures` en `CatalogService`.
- [x] **SeedController actualizado** (sesión 02/11/2026):
  - [x] Seed idempotente de `SizeGroups`, `Sizes`, `Features` y `FeatureOptions`.
  - [x] Claves compuestas de idempotencia (ej: `{FeatureId}_{Value.ToLower()}`).
  - [x] `SeedResultDto` ampliado con contadores de Features y FeatureOptions.

###  Pendiente (Frontend Blazor)
- [ ] **Maestros de catálogo en Blazor**:
  - [ ] Maestro de `SizeGroup` (CRUD simple).
  - [ ] Maestro de `Size` (con dropdown de `SizeGroup`).
  - [ ] Maestro de `Category` (con dropdown de `ParentId` para jerarquía).
  - [ ] Maestro de `Feature` (con gestión de `FeatureOptions` si es tipo Lista).
  - [ ] Maestro de `FeatureOption` (vinculado a `Feature`).
  - [ ] Maestro de `DataSource` (CRUD simple).
  - [ ] Maestro de `Marketplace` (CRUD simple).
- [ ] **Formulario de Producto en Blazor** (el "jefe final"):
  - [ ] Campos básicos (Sku, Nombre, Descripción, Precio base, Stock base).
  - [ ] Selector múltiple de Colores.
  - [ ] Selector múltiple de Categorías.
  - [ ] Tabla dinámica de Tallas (con Stock, Sku variante, Precio variante).
  - [ ] Sección de Marketplaces (con ExternalCode, LastSyncedAt).
  - [ ] Sección de DataSources (con ExternalCode, LastSyncedAt).
  - [ ] Sección dinámica de Características (InputText o Select según `DataType` de la Feature).

## Procesamiento asíncrono y tiempo real
- [x] Endpoint en backend para mejorar descripción de producto con IA (modelo ligero vía API) + Modal de confirmación en front + Toast de feedback (¡Hecho!)
- [x] SignalR implementado para notificaciones de tareas asíncronas (ej. enriquecimiento de descripciones)
- [ ] Importaciones masivas con progreso en tiempo real vía SignalR
- [ ] BackgroundServices para tareas periódicas
- [ ] Gestor de trabajos (Hangfire o similar)
- [ ] Redis para caché distribuido
- [ ] RabbitMQ para mensajería entre módulos (sincronización con marketplaces)
- [ ] Elasticsearch + Kibana para búsqueda y análisis de logs

## Integraciones y escalado
- [ ] API keys por cliente para integraciones externas (PrestaShop y similares)
- [ ] Evolución a multitenant (TenantId + filtros globales de EF + claim de tenant)
- [ ] UCP (Universal Commerce Protocol) como micro-proyecto de comercio agéntico

## Inteligencia Artificial y Búsqueda Avanzada
- [x] Búsqueda híbrida semántica (`pgvector` + filtros duros) con umbral de similitud configurable (ej. 60%) para descartar ruido
- [x] Extracción de intención de búsqueda en lenguaje natural mediante IA (traducción a JSON estructurado `SearchProductIntentDto`)
- [x] Seed de datos mejorado: generación de 200+ productos mock con idempotencia y descripciones ricas para embeddings de calidad
- [x] Extensión `VectorExtensions.CalculateCosineSimilarity` en `Shared` para cálculo seguro en memoria, evitando errores de traducción de EF Core
- [ ] (Futuro) Agente conversacional sobre la API (requerirá Semantic Kernel)
- [ ] (Futuro) Asistente integrado en el front Blazor

## Calidad y despliegue
- [ ] Tests unitarios (xUnit)
- [ ] Tests de integración con Testcontainers
- [ ] Tests de arquitectura
- [ ] Dockerización completa y CI/CD

## Deuda de front y UX (aparcados del chat)
- [x] Modal de confirmación Bootstrap para sustituir window.confirm en borrados (¡Hecho!)
- [x] Componente reutilizable NumberInput.razor (input numérico con validación + decimal locale-aware) (¡Hecho!)
- [x] Toast de errores auto-ocultables en grids y formularios (¡Hecho!)
- [ ] Refactor jerarquía PageBaseComponent / FormBaseComponent (grids vs formularios)
- [ ] Unificar estilo de rutas de endpoints (REST sustantivos vs verbos RPC)
- [ ] Si la consulta de producto crece, separarla a un componente ProductDetail propio

## Arquitectura y Evolución (NUEVO)
- [ ] **Migración a DDD canónico (4 capas)**:
  - Fase 1: Separar Domain de Data (pasar a modelo tipo "Store" de 3 capas)
  - Fase 2: Crear capa Application independiente (casos de uso, DTOs, interfaces)
  - Fase 3: Enriquecer Domain (mover reglas de FluentValidation a entidades)
  - Fase 4: (Opcional) CQRS con MediatR
- [ ] (Futuro) Evaluar migración a Servicio Centralizado de Notificaciones (SignalR + Redis/RabbitMQ) para desacoplar Hubs de APIs de negocio