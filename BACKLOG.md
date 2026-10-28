## Pendientes técnicos detectados
- [ ] Paginación real de servidor en la API (skip/take) cuando los datos crezcan
- [x] Creación de DTOs específicos para respuestas complejas (ej. `ProductSearchHybridDto`) evitando ensuciar entidades de dominio con datos ad-hoc (¡Hecho!)
- [ ] Limpieza C10: borrar `NavMenu.razor` y su css huérfanos, css viejos de la plantilla, splash de arranque del index.html
- [ ] Alta conjunta User+Person desde el front (formulario del perforador)
- [x] Validación de cliente con anotaciones + pintado de ProblemDetails del middleware en el front (¡Hecho!)

## Enriquecimiento del dominio

- [x] **Maestros base**: Entidades `DataSource` y `Marketplace` con CRUD completo (Backend + Frontend Blazor), validaciones y seeds iniciales.
### 🚧 En progreso (Plan estricto de hoy)
- [ ] **Categorías (Bloque fuerte)**:
  - [ ] Entidad `Category` con `ParentId` (jerarquía autorreferencial).
  - [ ] Tabla intermedia `ProductCategory` con campo `IsPrimary` (un producto puede tener varias, pero una principal).
  - [ ] Seed realista: "Ropa > Camisetas", "Calzado > Running", etc.
  - [ ] Actualizar entidad `Product` para relacionarse con `ProductCategory`.
- [ ] **Tallas (Si da tiempo)**:
  - [ ] Maestros: `SizeGroup` (ej. "EU", "US") y `Size` (ej. "42", "M").
  - [ ] Relación N:M: `CategorySizeGroup` (qué grupo de tallas aplica a qué categoría).
  - [ ] Entidad de unión: `ProductSize` con stock específico por talla.
- [ ] **Características (Mañana si no da tiempo)**:
  - [ ] Entidad `Attribute` vinculada a `Category` (ej. "Material", "Voltaje").
  - [ ] Entidad `ProductAttributeValue` para almacenar los valores concretos por producto.

## Procesamiento asíncrono y tiempo real
- [x] Endpoint en backend para mejorar descripción de producto con IA (modelo ligero vía API) + Modal de confirmación en front + Toast de feedback (¡Hecho!)
- [ ] Importaciones masivas con progreso en tiempo real vía SignalR
- [ ] BackgroundServices para tareas periódicas
- [ ] Gestor de trabajos (Hangfire o similar)
- [ ] Redis para caché distribuido
- [ ] RabbitMQ para mensajería entre módulos
- [ ] Elasticsearch + Kibana para búsqueda y análisis de logs

## Integraciones y escalado
- [ ] API keys por cliente para integraciones externas (PrestaShop y similares)
- [ ] Evolución a multitenant (TenantId + filtros globales de EF + claim de tenant)
- [ ] UCP (Universal Commerce Protocol) como micro-proyecto de comercio agéntico

## Inteligencia Artificial y Búsqueda Avanzada
- [x] Endpoint en backend para mejorar descripción de producto con IA (modelo ligero vía API) + Modal de confirmación en front (¡Hecho!)
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