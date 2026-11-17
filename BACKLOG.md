# BACKLOG - BackendMaster2

## 🗺️ Roadmap Estratégico Acordado
El desarrollo se ejecutará estrictamente en este orden, con estrategia **in crescendo** (de lo más simple y rápido a lo más complejo):

1. **Bloque 1: Cierre del PIM** (Seed jerárquico + Validación de obligatorias en backend + Imágenes: entidad + integración backend).
2. **Bloque 2: Infraestructura Asíncrona y Rendimiento** (Orden acordado: Redis caché → RabbitMQ + Excel → Hangfire → Shopify → WooCommerce).
3. **Bloque 3: Testing Ligero** (xUnit básico + Testcontainers, justo lo necesario para validar el Bloque 4).
4. **Bloque 4: Despliegue** (Docker Compose completo + CI/CD básico + README de alcance).
5. **Bloque 5: IA Avanzada** (Re-encuadre de búsqueda semántica al dominio actual + Agente conversacional / MCP).
6. **Complementario: Power Platform** (Fuera de los bloques principales, como extensión posterior al Bloque 2).

> ⚠️ **ALCANCE DEL PROYECTO**: BackendMaster2 es un **DEMOSTRADOR**, no un PIM operativo.
> No se implementan validaciones de frontend (la validación vive en backend).
> El README final debe declarar explícitamente lo que NO incluye.

---

## 📐 Principios Arquitectónicos (Instrucciones Permanentes)

1. **Api es delgada**: solo HTTP, JWT y delega.
2. **Modules contiene**: lógica, DTOs, interfaces y config externa (QwenSettings, AiService).
3. **Excepción**: JwtSettings y AuthService van en Api (seguridad transversal).
4. **Web es independiente**: no comparte código ni referencias con Api/Modules.
5. **Mapeo híbrido**: AutoMapper SOLO para mapeos planos (1 a 1). Los mapeos complejos con lógica condicional o proyecciones de EF Core se hacen a mano en services/repositories.
6. **Defensa en profundidad**: validación de negocio en backend (BusinessRuleException → 400). Frontend solo UX.
7. **Encapsulamiento entre módulos**: los services inyectan otros services (nunca repositorios de otros módulos).

---

## Pendientes técnicos detectados
- [ ] Paginación real de servidor en la API (skip/take) cuando los datos crezcan.
- [x] Creación de DTOs específicos para respuestas complejas (ej. `ProductSearchHybridDto`).
- [ ] Limpieza C10: borrar `NavMenu.razor` y su css huérfanos, css viejos de la plantilla, splash de arranque del index.html.
- [ ] Alta conjunta User+Person desde el front (formulario del perforador).
- [x] Validación de cliente con anotaciones + pintado de ProblemDetails del middleware en el front.
- [ ] **Deuda de mapeo**: aplicar filosofía híbrida a `ProductDetailDto` → `ProductDto` en el frontend (el mapeo manual campo a campo ya provocó un olvido con DataSources/Marketplaces).
- [x] **Bug de serialización de enums**: resuelto con fallback seguro en AutoMapper (`CatalogProfile`).
- [x] **Persistencia `FeatureOptionId`**: resuelto (se guardaba en el DTO pero no se copiaba a la entidad en Create/Update).

---

## 🟢 Bloque 1: Cierre del PIM

### ✅ Completado (Backend)
- [x] Maestros base: `DataSource` y `Marketplace` con CRUD, validaciones y seeds.
- [x] Categorías: `Category`, `CategoryFeature` (N:M con `IsRequired`), seed e integración.
- [x] **Jerarquía de Categorías (Modelo)**: `ParentId` con `OnDelete(Restrict)`.
- [x] Tallas: `SizeGroup`, `Size`, `ProductSize` con payload, integración en `ProductService`.
- [x] Relaciones N:M en Product: Patrón quirúrgico (`Clear()` + `Add()`) en las 5 relaciones.
- [x] Atributos Flexibles (Features): Modelo EAV relacional + `FeatureOptionId`.
- [x] SeedController: Seed idempotente de SizeGroups, Sizes, Features, FeatureOptions, CategoryFeatures.
- [x] **Endpoint `GetMandatoryFeatures`**: `GET /api/category/GetMandatoryFeatures/{id}`.
- [x] **Validación de obligatorias en backend**: `ValidateProductAsync` en `ProductService` (Create/Update), comprueba existencia + valor (`Value` o `FeatureOptionId`) de las obligatorias de la categoría.
- [x] **`BusinessRuleException`**: nueva excepción en `Shared.Common` (hereda de `DomainException`), traducida a 400 por el `ExceptionMiddleware`.
- [x] **Entidad `ProductImage`**: creada en dominio con campos `StorageKey`, `Url`, `AltText`, `Order`, `IsPrimary`, `Source`, `ExternalId`, `CreatedAt`.
- [x] **Migración `AddImages`**: aplicada correctamente (tabla `ProductImages` con FK en cascada e índice compuesto `ProductId + Order`).
- [x] **Integración en `ProductService`**: patrón `Clear() + Add()` aplicado a `ProductImages` en `CreateAsync` y `UpdateAsync`.

### ✅ Completado (Frontend Blazor)
- [x] Menú lateral jerárquico (Opción B).
- [x] Maestros de catálogo: SizeGroup, Size, Category, Feature, DataSource, Marketplace.
- [x] `ProductForm.razor`: DTOs aplanados, Colores, Variantes, Canales (propiedad puente).
- [x] Pestaña de Características: renderizado dinámico por `DataType`.
- [x] Select de Categoría única con propiedad puente (`SelectedCategoryId`).
- [x] **Lógica de cambio de categoría**: `ConfirmModal` + `@onchange` + `_pendingCategoryId` + auto-carga de obligatorias desde el endpoint.
- [x] **Persistencia correcta de features tipo Lista**: `FeatureOptionId` viaja y se guarda.

### ❌ Descartado por alcance (NO se hace)
- [ ] ~~Validación de características obligatorias en front (`FormValidation`)~~.
      Motivo: es un demostrador; la validación vive en backend. Documentar en README.

### ⏳ Pendiente (Bloque 1 - Mínimo Viable)
- [ ] **Mensajes amables de error al borrar**: capturar FK `Restrict` al borrar una categoría con hijas y devolver mensaje claro (no error crudo de PostgreSQL).
- [ ] **Seed jerárquico realista**: `SeedController` con subcategorías (`ParentId`) y `CategoryFeatures` obligatorias coherentes.

### ⏳ Imágenes: Backend (DTOs y mapeo)
- [ ] **`ProductImageDto`** en `BackendMaster2.Modules/Dtos/` (misma estructura que la entidad).
- [ ] **`ProductImageDto`** en `BackendMaster2.Web/Models/Dtos/` (para el cliente Blazor).
- [ ] **Propiedad `ProductImages`** en `ProductDto` y `ProductDetailDto` (backend y frontend).
- [ ] **Mapeo AutoMapper plano** en `CatalogProfile`: `CreateMap<ProductImage, ProductImageDto>()` y reverso con `.ForMember(dest => dest.Product, opt => opt.Ignore())`.
- [ ] **Mapeo manual en `ProductService`** para `Product → ProductDto` (aplicando filosofía híbrida: las colecciones anidadas se mapean a mano, no por AutoMapper).

### ⏳ Imágenes: Frontend (aplazado a brake de desarrollo asíncrono)
- [ ] Pestaña de Imágenes en `ProductForm.razor`: listado ordenado, marcado de principal, subida/URL.
- [ ] Integración futura con MinIO/S3 (cuando se levante el contenedor).

### ⏳ UI de Jerarquía de Categorías (Aplazado intencionadamente)
- [ ] `CategoryForm`: select de "categoría padre" excluyendo el nodo y sus descendientes.
- [ ] `ProductForm`: helper de aplanado pre-orden mostrando ruta (`Padre › Hijo`).
- [ ] Decisión: ¿en `ProductForm` se selecciona cualquier nivel o solo hojas?

### ⏳ Otros pendientes de Front
- [ ] `Products.razor`: Grid de productos con filtros y ordenación (pulir).

---

## 🟡 Bloque 2: Infraestructura Asíncrona y Rendimiento

### ✅ Completado
- [x] Endpoint IA para mejorar descripción + Modal + Toast.
- [x] SignalR para notificaciones de tareas asíncronas.

### Fase 2.1: Caché Distribuida (Redis) ⬅️ **PRÓXIMO**
*Objetivo: aliviar la carga de BBDD en el `ProductForm` (hoy hace 6-7 llamadas de maestros al abrir).*
- [ ] **Levantar contenedor Redis** en `docker-compose`.
- [ ] **Alcance de caché**: estrictamente Maestros del Catálogo (Colores, Categorías, Tallas, Grupos de Tallas, Features).
- [ ] **Invalidación específica**: al crear/editar/borrar un maestro, borrar solo su clave de caché (no toda la caché).
- [ ] **Utilidades adicionales de Redis** (a valorar según tiempo):
  - [ ] **Rate Limiting** en la API (contador por usuario/token con `INCR` + `EXPIRE`).
  - [ ] **Distributed Locks** para el Worker de RabbitMQ (evitar procesamiento duplicado de mensajes).
  - [ ] **Backend de Hangfire** (más rápido que PostgreSQL para estado de jobs).

### Fase 2.2: Procesamiento Asíncrono Básico (RabbitMQ + Workers)
*Caso de uso real: importación masiva de Excel. Sin dependencias externas.*
- [ ] **Levantar contenedor RabbitMQ** en `docker-compose`.
- [ ] **Contrato del mensaje**: decidir si viaja el Excel en bruto (base64) o ya parseado a DTOs (impacta tamaño de mensaje y acoplamiento).
- [ ] **Endpoint de importación** (API delgada): recibe Excel, lo encola, devuelve `202 Accepted` (como ya hace la IA).
- [ ] **Background Worker**: proyecto independiente o integrado, que lea de `excel_import_queue`, procese fila a fila, guarde productos y notifique por SignalR.
- [ ] **Patrón Strategy (Preparación)**: interfaces `IMarketplaceSyncService` e `IDataSourceSyncService` para fases posteriores sin tocar el core.
- [ ] **Distributed Locks con Redis** (opcional): proteger el worker contra reprocesamiento de mensajes.

### Fase 2.3: Monitorización de Jobs (Hangfire)
- [ ] **Integración de Hangfire**: NuGet + dashboard en `/hangfire`.
- [ ] **Backend Redis para Hangfire** (si se implementa en Fase 2.1).
- [ ] **Migración de Workers a Hangfire**: sustituir/complementar BackgroundServices.
- [ ] **Monitorización de sincronizaciones**: estado de jobs de sync + reintentos.

### Fase 2.4: Integración Real con Shopify (DataSource)
- [ ] `ShopifySyncService` contra Development Store gratuita.
- [ ] Mapeo Shopify → PIM (`ProductDto` + `ProductImage` con `Source = "Shopify"`).
- [ ] Encolado en RabbitMQ/Hangfire (no bloquea UI).
- [ ] Actualización de `LastSyncedAt` en `ProductDataSource`.

### Fase 2.5: Integración Real con WooCommerce (Marketplace)
- [ ] Docker Compose para WooCommerce (WordPress + Woo + MySQL).
- [ ] `WooCommerceSyncService` contra API REST (`POST /wp-json/wc/v3/products`).
- [ ] Mapeo PIM → WooCommerce (incluyendo imágenes).
- [ ] Gestión de errores y reintentos desde Hangfire.
- [ ] Actualización de `LastSyncedAt` en `ProductMarketplace`.

### Fase 2.6: Storage de Imágenes (MinIO/S3)
*Nota: la entidad y migración ya están en Bloque 1. Aquí se implementa el backend de subida.*
- [ ] **Levantar contenedor MinIO** en `docker-compose` (compatible S3).
- [ ] **Servicio `IStorageService`** en Modules: interfaz abstracta contra S3 (SDK `AWSSDK.S3`), apuntando a MinIO local.
- [ ] **Endpoint de subida** en API: recibe archivo, lo sube a MinIO, devuelve `StorageKey` + `PublicUrl`.
- [ ] **Worker de procesamiento de imágenes** (opcional): descarga URL externa, redimensiona con `ImageSharp`, sube a MinIO.
- [ ] **Frontend**: pestaña de imágenes en `ProductForm` con carrusel, marcado de principal, subida.

### Fase 2.7: Otras integraciones asíncronas (Opcional)
- [ ] Importaciones masivas con progreso en tiempo real vía SignalR.
- [ ] Elasticsearch + Kibana para búsqueda y logs.

---

## 🟠 Bloque 3: Calidad y Testing (Ligero)
*Objetivo: Asegurar lo justo y necesario para dar confianza al Bloque 4.*
- [ ] Tests unitarios (xUnit) para la validación de obligatorias en `ProductService`.
- [ ] Tests de integración con Testcontainers (PostgreSQL) para Seed jerárquico y migraciones.
- [ ] (Opcional) Tests de arquitectura básicos con ArchUnitNET (ej: "Los Controllers no pueden inyectar DbContext directamente").

---

## 🔵 Bloque 4: Despliegue y Producción
- [ ] **Docker Compose completo**: API + PostgreSQL + RabbitMQ + Redis + MinIO + Hangfire (+ WooCommerce opcional).
- [ ] **CI/CD**: Pipeline básico con GitHub Actions (Build + Test + Push imagen Docker).
- [ ] **README de alcance**: documentar que es un DEMOSTRADOR, no un PIM operativo.
      Declarar lo que NO incluye: validaciones de front, multidioma, multimoneda,
      gestión de imágenes/assets (parcial), versionado/audit, workflows de aprobación,
      concurrencia optimista, RBAC fino, paginación real.
- [ ] **Monitoreo básico**: logs centralizados con Serilog.

---

## 🟣 Bloque 5: IA y Búsqueda Avanzada
*⚠️ Intercalar con Bloque 2: la búsqueda se quedó corta al crecer el dominio (solo contemplaba colores).*

### ✅ Completado
- [x] Búsqueda híbrida semántica (`pgvector` + filtros duros) con umbral configurable.
- [x] Extracción de intención de búsqueda con IA.
- [x] Seed de 200+ productos mock con descripciones ricas.

### ⏳ Re-encuadre de la búsqueda al modelo de dominio actual
- [ ] **Extracción de intención ampliada**: `GenerateProductSearchIntent` debe extraer también categoría (resolviendo descendientes), talla y features (ej: "abrigo de lana para hombre talla L").
- [ ] **Embedding enriquecido del producto**: hoy solo se genera de la descripción. Ampliar a nombre + categoría + features para que la similitud semántica sea real.
- [ ] **Filtros duros ampliados** en el repositorio: categoría, talla en stock, valores de features.
- [ ] (Futuro) Agente conversacional sobre la API (Semantic Kernel).
- [ ] (Futuro) Asistente integrado en el front Blazor.

---

## 🚀 Exploración Power Platform (Complementario / Post-Bloque 2)
*Nota: NO integrar en los bloques principales para evitar desvío de foco.*
- [ ] **Power App de consulta**: Canvas App consumiendo la API para listar y filtrar productos.
- [ ] **Power Automate**: flujo que escuche Webhook "ProductoCreado" → email/SharePoint.
- [ ] **Power BI**: conexión a PostgreSQL → dashboard de métricas (stock por categoría, productos pendientes de sync).
- [ ] **Copilot Studio**: agente conversacional sobre Power Platform.
- [ ] Documentar en README como "Extensibilidad Enterprise".

---

## 🎨 Mejoras de UX aplazadas (Backlog de UI)
*Se posponen indefinidamente a menos que sobre tiempo tras completar los Bloques 1 y 2.*
- [ ] Chip/Tag Input para creación en lote de tallas desde `SizeGroup`.
- [ ] Refactor jerarquía `PageBaseComponent` / `FormBaseComponent`.
- [ ] Unificar estilo de rutas de endpoints (REST sustantivos vs verbos RPC).

---

## 🏛️ Arquitectura y Evolución (Futuro)
- [ ] Migración a DDD canónico (4 capas): separar Domain de Data, crear capa Application independiente.
- [ ] Evaluar migración a Servicio Centralizado de Notificaciones (SignalR + Redis/RabbitMQ) para desacoplar Hubs de APIs de negocio.
- [ ] UCP (Universal Commerce Protocol) como micro-proyecto de comercio agéntico.
- [ ] **Filosofía de mapeo consolidada**: AutoMapper solo para mapeos planos (1 a 1);
      mapeos complejos con lógica condicional o proyecciones de EF Core, a mano en services/repositories.