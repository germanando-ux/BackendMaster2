# BACKLOG - BackendMaster2

## 🗺️ Roadmap Estratégico Acordado
El desarrollo se ejecutará estrictamente en este orden, con estrategia **in crescendo** (de lo más simple y rápido a lo más complejo):
1. **Bloque 1: Cierre del PIM** (Seed jerárquico + Validación de obligatorias en backend + mínimos de front).
2. **Bloque 2: Infraestructura Asíncrona y Rendimiento** (Estrategia in crescendo en 5 fases).
3. **Bloque 3: Testing Ligero** (xUnit básico + Testcontainers, justo lo necesario para validar el Bloque 4).
4. **Bloque 4: Despliegue** (Docker Compose completo + CI/CD básico).
5. **Bloque 5: IA Avanzada** (Solo si sobra tiempo: Agente conversacional / MCP).
6. **Complementario: Power Platform** (Fuera de los bloques principales, como extensión o proyecto paralelo posterior al Bloque 2).

---

## Pendientes técnicos detectados
- [ ] Paginación real de servidor en la API (skip/take) cuando los datos crezcan.
- [x] Creación de DTOs específicos para respuestas complejas (ej. `ProductSearchHybridDto`) evitando ensuciar entidades de dominio.
- [ ] Limpieza C10: borrar `NavMenu.razor` y su css huérfanos, css viejos de la plantilla, splash de arranque del index.html.
- [ ] Alta conjunta User+Person desde el front (formulario del perforador).
- [x] Validación de cliente con anotaciones + pintado de ProblemDetails del middleware en el front.
- [ ] **Deuda de mapeo**: Evaluar mapeo automático o fábrica para `ProductDetailDto` → `ProductDto` en el frontend (el mapeo manual campo a campo ya provocó un olvido con DataSources/Marketplaces).

---

## 🟢 Bloque 1: Cierre del PIM (Inmediato)

### ✅ Completado (Backend)
- [x] Maestros base: `DataSource` y `Marketplace` con CRUD completo, validaciones y seeds.
- [x] Categorías: Entidad `Category`, tabla intermedia `CategoryFeature` (N:M con `IsRequired`), seed realista e integración en `CatalogService` con patrón quirúrgico.
- [x] **Jerarquía de Categorías (Modelo)**: Añadido `ParentId` (Guid? auto-FK) con `OnDelete(Restrict)`. El campo viaja en los DTOs, pero la UI de gestión jerárquica se aplaza (ver sección Frontend).
- [x] Tallas: Maestros `SizeGroup` y `Size`, entidad de unión `ProductSize` con payload (`Stock`, `Sku`, `Price`), e integración en `ProductService`.
- [x] Integración de relaciones N:M en Product: Patrón quirúrgico (`Clear()` + `Add()`) aplicado a las 5 relaciones (Colores, Categorías, Tallas, DataSources, Marketplaces).
- [x] Atributos Flexibles (Features): Modelo EAV relacional (`Feature`, `FeatureOption`, `ProductFeature`, `CategoryFeature`). Refactorización a clave subrogada (`Id`) y adición de `FeatureOptionId` para integridad referencial en tipos Lista.
- [x] SeedController actualizado: Seed idempotente de SizeGroups, Sizes, Features, FeatureOptions y asignación de CategoryFeatures.

### ✅ Completado (Frontend Blazor)
- [x] Menú lateral jerárquico (Opción B) con despliegue/colapso por estado C#.
- [x] Maestros de catálogo: `SizeGroup`, `Size`, `Category` (Master-Detail), `Feature` (con gestión condicional de opciones), `DataSource` y `Marketplace`.
- [x] `ProductForm.razor` (Base operativa): Sincronizado con DTOs aplanados, selector de Colores y pestañas de Variantes y Canales (con propiedad puente para origen único y tabla dinámica de marketplaces).
- [x] Pestaña de Características: Renderizado dinámico de controles (Texto, Número, Switch, Lista) basado en `DataType`, usando helper de extensiones y lookup de opciones desde `availableFeatures`.
- [x] Select de Categoría única: Implementado con propiedad puente (`SelectedCategoryId`) preparando el terreno para la jerarquía.

### ⏳ Pendiente (Bloque 1 - Mínimo Viable)
- [ ] **Seed jerárquico realista**: Actualizar el `SeedController` para crear categorías con subcategorías (usando el nuevo `ParentId`) y asignar `CategoryFeatures` marcadas como `IsRequired = true` de forma coherente.
- [ ] **Validación de obligatorias en backend**: En `ProductService` (Create/Update), verificar que todas las `Feature` asociadas a la categoría del producto con `IsRequired = true` tengan un `Value` o `FeatureOptionId` informado en el DTO. Devolver `ProblemDetails` si falta alguna.
- [ ] **Mensajes amables de error**: Capturar la excepción de FK al borrar una categoría con hijas (Restrict) y devolver un mensaje claro en lugar del error crudo de PostgreSQL.
- [ ] **Validación de características obligatorias en front**: En `FormValidation`, comprobar que las features asignadas como obligatorias por la categoría tengan valor antes de permitir el guardado.
- [ ] **Lógica de cambio de categoría**: Implementar `ConfirmModal` al cambiar de categoría en el formulario: limpiar `ProductFeatures` y auto-añadir las obligatorias de la nueva categoría seleccionada.
- [ ] **Ajuste del Seed**: Verificar y ampliar el seed de `CategoryFeatures` para que asigne características realistas y obligatorias a las categorías de prueba, permitiendo validar la carga automática en el frontend.

### ⏳ UI de Jerarquía de Categorías (Aplazado intencionadamente)
- [ ] `CategoryForm`: Select de "categoría padre" excluyendo el propio nodo y sus descendientes.
- [ ] `ProductForm`: Evolucionar el select plano a un helper de aplanado pre-orden que muestre la ruta completa (`Padre › Hijo`).
- [ ] Decisión pendiente: ¿en `ProductForm` se puede seleccionar cualquier nivel o solo hojas?

### ⏳ Otros pendientes de Front
- [ ] `Products.razor`: Grid de productos con filtros y ordenación (pendiente de pulir).

---

## 🟡 Bloque 2: Infraestructura Asíncrona y Rendimiento (Estrategia in crescendo)

### ✅ Completado
- [x] Endpoint en backend para mejorar descripción de producto con IA + Modal de confirmación en front + Toast de feedback.
- [x] SignalR implementado para notificaciones de tareas asíncronas.

### Fase 2.1: Procesamiento Asíncrono Básico (RabbitMQ + Workers)
*Caso de uso rápido y realista: importación masiva de Excel. Sin dependencias externas.*
- [ ] **Caso de uso real (Importación Excel)**: Crear un endpoint que reciba un archivo Excel de productos. En lugar de procesarlo, serializarlo y enviarlo a una cola de RabbitMQ (`excel_import_queue`).
- [ ] **Background Worker**: Crear un servicio en segundo plano que lea de la cola, procese el Excel fila a fila, guarde los productos en la BBDD y actualice el estado (usando SignalR para avisar al front cuando termine, que ya tenemos implementado).
- [ ] **Patrón Strategy (Preparación)**: Dejar la arquitectura lista con interfaces `IMarketplaceSyncService` e `IDataSourceSyncService` para que en fases posteriores se puedan añadir integraciones reales (Shopify/WooCommerce) sin tocar el core.

### Fase 2.2: Optimización de Rendimiento (Redis)
*Optimización de lo que ya tenemos: el formulario de producto es "pesadito" por las múltiples consultas de maestros.*
- [ ] **Caché de Maestros**: Implementar `IDistributedCache` con Redis para almacenar las listas de Colores, Categorías, Tallas y Features.
- [ ] **Invalidación de caché**: Asegurar que cuando se cree o edite un maestro (ej. un nuevo Color), se borre de la caché para que la próxima vez se traiga el actualizado de la BBDD.
- [ ] **Optimización del ProductForm**: Aplicar la caché a los endpoints que carga el formulario de producto para reducir drásticamente el tiempo de respuesta.

### Fase 2.3: Monitorización de Jobs (Hangfire)
*Dashboard web para ver el estado de los trabajos encolados. Opcional pero muy vistoso para portfolio.*
- [ ] **Integración de Hangfire**: Añadir paquete NuGet y configurar dashboard en `/hangfire`.
- [ ] **Migración de Workers a Hangfire**: Sustituir (o complementar) los BackgroundServices manuales por jobs de Hangfire para las tareas de importación Excel.
- [ ] **Monitorización de sincronizaciones**: Usar Hangfire para registrar el estado de los jobs de sync (éxito, fallo, reintentos) y poder reintentar jobs fallidos con un clic desde el dashboard.

### Fase 2.4: Integración Real con Shopify (DataSource)
*Conectar el PIM con una Development Store gratuita de Shopify para importar productos reales.*
- [ ] **Implementación de `ShopifySyncService`**: Servicio que llame a la API REST de Shopify (`GET /admin/api/2024-01/products.json`) usando credenciales de una Development Store gratuita.
- [ ] **Mapeo de datos Shopify → PIM**: Transformar los productos de Shopify (título, precio, stock, variantes, imágenes) al modelo de `ProductDto` de BackendMaster2.
- [ ] **Encolado en RabbitMQ/Hangfire**: La importación desde Shopify se encola como un job asíncrono, no bloquea la UI.
- [ ] **Actualización de `LastSyncedAt`**: Tras la sync exitosa, actualizar el campo de trazabilidad en `ProductDataSource`.

### Fase 2.5: Integración Real con WooCommerce (Marketplace)
*Publicar productos del PIM en un WooCommerce local levantado con Docker (100% gratis, sin licencias).*
- [ ] **Docker Compose para WooCommerce**: Añadir al `docker-compose.yml` un contenedor con WordPress + WooCommerce + MySQL, con productos de ejemplo importados desde CSV.
- [ ] **Implementación de `WooCommerceSyncService`**: Servicio que llame a la API REST de WooCommerce (`POST /wp-json/wc/v3/products`) usando API keys generadas en el panel de admin.
- [ ] **Mapeo de datos PIM → WooCommerce**: Transformar `ProductDto` al formato que espera WooCommerce (nombre, descripción, precio, stock, categorías, imágenes por URL).
- [ ] **Gestión de errores y reintentos**: Si la API de WooCommerce responde con error (ej: producto duplicado), registrar el error y permitir reintento manual desde Hangfire.
- [ ] **Actualización de `LastSyncedAt`**: Tras la publicación exitosa, actualizar el campo de trazabilidad en `ProductMarketplace`.

### Fase 2.6: Otras integraciones asíncronas (Opcional)
- [ ] **Importaciones masivas con progreso en tiempo real vía SignalR**: Mejorar la UX de la importación Excel mostrando progreso en tiempo real (ej: "Procesando fila 45 de 200...").
- [ ] **Elasticsearch + Kibana para búsqueda y análisis de logs**: Centralizar logs de la API y de los workers para monitorización avanzada.

---

## 🟠 Bloque 3: Calidad y Testing (Ligero)
*Objetivo: Asegurar lo justo y necesario para dar confianza al Bloque 4.*
- [ ] Tests unitarios (xUnit) para la validación de características obligatorias en `ProductService`.
- [ ] Tests de integración con Testcontainers (PostgreSQL) para verificar que el Seed jerárquico y las migraciones se aplican correctamente en un entorno limpio.
- [ ] (Opcional) Tests de arquitectura básicos con ArchUnitNET (ej: "Los Controllers no pueden inyectar DbContext directamente").

---

## 🔵 Bloque 4: Despliegue y Producción
- [ ] **Docker Compose completo**: Orquestación de API + PostgreSQL + RabbitMQ + Redis + Hangfire + (Opcional: contenedor de WooCommerce para demo).
- [ ] **CI/CD**: Pipeline básico con GitHub Actions (Build + Test + Push de imagen Docker).
- [ ] **Documentación**: README detallado sobre cómo levantar el entorno de demo con un solo comando (`docker-compose up`).
- [ ] **Monitoreo básico**: Logs centralizados con Serilog.

---

## 🟣 Bloque 5: Inteligencia Artificial y Búsqueda Avanzada (Opcional)
*Ya implementado: Búsqueda híbrida semántica (pgvector), extracción de intención con IA, Seed de 200+ productos mock.*
- [x] Búsqueda híbrida semántica (`pgvector` + filtros duros) con umbral de similitud configurable.
- [x] Extracción de intención de búsqueda en lenguaje natural mediante IA.
- [x] Seed de datos mejorado: generación de 200+ productos mock con descripciones ricas para embeddings de calidad.
- [ ] (Futuro) Agente conversacional sobre la API (Semantic Kernel).
- [ ] (Futuro) Asistente integrado en el front Blazor.

---

## 🚀 Exploración Power Platform (Complementario / Post-Bloque 2)
*Nota: NO integrar en los bloques principales para evitar desvío de foco. El objetivo es posicionarse como ".NET Backend Developer con capacidad de extender con Power Platform", no como desarrollador RPA/UiPath puro.*
- [ ] **Power App de consulta**: Crear una app simple (Canvas App) que consuma la API de BackendMaster2 para listar y filtrar productos.
- [ ] **Power Automate**: Crear un flujo que escuche un Webhook de la API (ej: "ProductoCreado") y envíe una notificación por email o registre el evento en un Excel de SharePoint.
- [ ] **Power BI**: Conectar directamente a la base de datos PostgreSQL del proyecto para crear un dashboard de métricas (stock por categoría, productos pendientes de sincronizar).
- [ ] **Copilot Studio**: Explorar agente conversacional sobre Power Platform (Microsoft está empujando fuerte aquí).
- [ ] Documentar esta integración en el README como "Extensibilidad Enterprise".

---

## 🎨 Mejoras de UX aplazadas (Backlog de UI)
*Se posponen indefinidamente a menos que sobre tiempo tras completar los Bloques 1 y 2.*
- [ ] Chip/Tag Input para creación en lote de tallas desde `SizeGroup` (escribe + Enter/coma → chip con ✕). *Decisión de patrón: Chips para maestros simples, tabla dinámica para relaciones con payload.*
- [ ] Refactor jerarquía `PageBaseComponent` / `FormBaseComponent` (grids vs formularios).
- [ ] Unificar estilo de rutas de endpoints (REST sustantivos vs verbos RPC).

---

## 🏛️ Arquitectura y Evolución (Futuro)
- [ ] Migración a DDD canónico (4 capas): Separar Domain de Data, crear capa Application independiente.
- [ ] Evaluar migración a Servicio Centralizado de Notificaciones (SignalR + Redis/RabbitMQ) para desacoplar Hubs de APIs de negocio.
- [ ] UCP (Universal Commerce Protocol) como micro-proyecto de comercio agéntico.