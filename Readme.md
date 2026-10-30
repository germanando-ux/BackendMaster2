# BackendMaster2

Proyecto de aprendizaje progresivo para consolidar conocimientos de .NET, arquitectura modular y desarrollo full-stack con Blazor WebAssembly.

## 🎯 Objetivo

Construir una aplicación completa (backend + frontend) que sirva como portfolio técnico, explorando patrones de arquitectura modernos mientras se aprenden conceptos nuevos paso a paso.

## 📊 Estado actual

**Bloque A (Backend):** ✅ Completado
- API REST con ASP.NET Core
- Autenticación JWT con refresh tokens
- PostgreSQL dockerizado + EF Core
- Middleware de excepciones con ProblemDetails
- Validación con FluentValidation
- Scalar/OpenAPI para documentación interactiva
- CORS configurado para desarrollo
- ✅ Búsqueda híbrida semántica con `pgvector` e IA (extracción de intención + umbral de similitud)
- ✅ **Dominio de Producto evolucionado a PIM**: Relaciones N:M con payload implementadas (Tallas con Stock/Precio, Categorías, DataSources y Marketplaces). Patrón de sincronización quirúrgica (Clear + Add) aplicado en EF Core para actualizaciones eficientes y transaccionales.
- ✅ **SignalR**: Implementado para notificaciones de progreso en tiempo real (ej. enriquecimiento de descripciones con IA).

**Bloque C (Frontend):**  En progreso
- Blazor WebAssembly standalone
- Bootstrap como única librería de UI (MudBlazor evaluado y retirado)
- Layout propio: sidebar colapsable, navbar y tema de marca
- Página de productos con listado, estados de carga/error y tabla tematizada

## 🛠️ Stack tecnológico

### Backend
- **.NET 8** con ASP.NET Core
- **Entity Framework Core** + PostgreSQL dockerizado
- **Npgsql + Pgvector**: Búsqueda vectorial y operadores de distancia de coseno (`<=>`)
- **Microsoft.Extensions.AI**: Extracción de intención de búsqueda en lenguaje natural y generación de embeddings
- **FluentValidation** para validación de entrada
- **BCrypt** para hashing de contraseñas
- **JWT Bearer** para autenticación
- **Scalar** para documentación OpenAPI interactiva
- **SignalR** para comunicación en tiempo real servidor-cliente

### Frontend
- **Blazor WebAssembly** (SPA con C#)
- **Bootstrap** (para componentes HTML personalizados)

## 🏗️ Decisiones de arquitectura

### 1. DTOs en cliente y servidor (estrategia de espejo manual)
**Decisión:** Los DTOs existen tanto en el backend como en el frontend, copiados manualmente.
**Por qué esta elección:** El frontend vive en la misma solución pero es una isla independiente. Evita acoplar el Web al proyecto Api (no arrastrar dependencias del servidor) y permite segregar el frontend a otro repositorio en el futuro sin romper nada. El coste de mantener los espejos es asumible y predecible.

### 2. Uso flexible de entidades vs DTOs
**Decisión:** En algunos endpoints simples se devuelven entidades directamente, mientras que en endpoints complejos se usan DTOs específicos.
**Por qué esta flexibilidad:** Para endpoints de lectura simple, crear un DTO idéntico a la entidad añade fricción sin beneficio claro. Los DTOs se usan cuando hay transformación de datos, ocultación de campos internos, o composición de múltiples fuentes.

### 3. Controllers REST pragmáticos
**Decisión:** Los controllers siguen convenciones REST en general, pero priorizan claridad y simplicidad sobre adherencia estricta al patrón.
**Por qué esta flexibilidad:** Para peticiones complejas, REST puro se queda corto. Reduce la carga cognitiva al navegar el código.

### 4. Blazor WebAssembly con Bootstrap puro
**Decisión:** El frontend usa Bootstrap + HTML puro como única librería de UI.
**Por qué esta elección:** Control total del CSS, menos magia, aprendizaje más profundo de los mecanismos de Blazor sin capas intermedias y cero dependencias de UI externas que versionar.

### 5. Autenticación JWT
JWT con access tokens cortos (15 min) y refresh tokens en base de datos (7 días) con capacidad de revocación. Patrón estándar de la industria.

### 6. Arquitectura modular en el backend
**Decisión:** El código está organizado en módulos (`Auth`, `ProductManagement`) dentro de `BackendMaster2.Modules`, no en carpetas por capa técnica.
**Por qué esta estructura:** Cada módulo es autocontenido, facilita la extracción futura a microservicios, reduce el acoplamiento y hace el código más fácil de navegar.

### 7. Búsqueda Híbrida y Umbral de Similitud
**Decisión:** La búsqueda combina filtros duros en la base de datos con ordenación semántica (`pgvector`), aplicando un umbral de similitud en memoria para descartar resultados irrelevantes.
**Por qué esta elección:** Evita devolver "lo menos malo" cuando no hay coincidencias reales. El cálculo del score se delega a C# puro para evitar errores de traducción de EF Core.

### 8. Evolución hacia PIM: Pragmatismo sobre Sobreingeniería
**Decisión:** El modelo de datos evoluciona hacia un PIM (Product Information Management) simplificado. Se gestionan orígenes (`ProductDataSource`) y canales de destino (`ProductMarketplace`) con atributos específicos de sincronización (`ExternalCode`, `LastSyncedAt`), evitando la complejidad extrema de sistemas enterprise legacy.
**Contexto:** Un PIM real (como los utilizados en integración con grandes marketplaces) suele tener una complejidad abrumadora: multi-idioma, multi-moneda, gestión de assets digitales, reglas de negocio por canal y separación estricta de "características de cliente" vs "características de proveedor". 
**Por qué esta elección:** 
- **YAGNI:** Replicar esa complejidad en un proyecto de aprendizaje añade coste sin valor formativo inmediato.
- **Escalabilidad controlada:** El modelo actual (entidad central `Product` + tablas de unión con payload) es suficiente para demostrar la gestión de variantes y la distribución multicanal.
- **Futuro:** Si se requieren atributos específicos por canal, el sistema de Atributos (Bloque E) está diseñado para extenderse sin necesidad de duplicar toda la estructura de tablas.

## 🗺️ Roadmap

### Completado
- ✅ **Bloque A:** Backend completo con autenticación JWT, gestión de productos, middleware de errores y búsqueda semántica.
- ✅ **Bloque A.5:** Validación JWT en endpoints protegidos.
- ✅ **Configuración:** PostgreSQL dockerizado, EF Core, migraciones, seeds.
- ✅ **Comunicación en tiempo real:** SignalR implementado para notificaciones de tareas asíncronas.

### En progreso
- 🚧 **Bloque C:** Frontend Blazor WebAssembly
  - ✅ Esqueleto de layout con Bootstrap 
  - ✅ Configuración CORS en backend
  - ✅ Integración con API de autenticación
  - ✅ Gestión de estado de sesión
  - ⏳ Páginas de productos y usuarios

### Futuro
- ⏳ **Bloque D:** Procesamiento asíncrono y tareas en segundo plano
  - **BackgroundServices** para tareas periódicas y limpieza automática
  - **Gestor de trabajos** (Hangfire o similar) para colas de trabajo persistentes
  - **Batch processing** para importaciones masivas de productos
  - **Redis** para caché distribuido y rate limiting
  - **RabbitMQ** para mensajería asíncrona entre módulos (sincronización con marketplaces)
  - **Elasticsearch + Kibana** para búsqueda avanzada de productos y análisis de logs

-  **Bloque E:** Enriquecimiento del dominio PIM
  - ✅ Categorías de producto (maestro nuevo + relación N:M)
  - ✅ Tallas (maestro + relación N:M con payload de Stock, Sku y Price)
  - ✅ Canales y Orígenes (`ProductMarketplace` y `ProductDataSource` con trazabilidad de sincronización)
  -  **Atributos flexibles:** Diseño e implementación (JSONB vs EAV) para características dinámicas, paso previo imprescindible para la sincronización batch con RabbitMQ.
  - ⏳ Actualización del Seed para poblar relaciones complejas antes de la integración total con el Frontend.

- ⏳ **Bloque F:** Testing y calidad
  - Tests unitarios con xUnit
  - Tests de integración con Testcontainers (PostgreSQL real en Docker)
  - Tests de arquitectura con ArchUnitNET
  - Cobertura de código con ReportGenerator

- ⏳ **Bloque G:** Despliegue y producción
  - Dockerización completa con Docker Compose
  - CI/CD con GitHub Actions
  - Despliegue en Azure/AWS
  - Monitoreo y logging centralizado

- ⏳ **Bloque H:** Inteligencia Artificial con el stack de Microsoft
  - **Semantic Kernel** como orquestador de agentes
  - **Microsoft.Extensions.AI** como abstracción de modelos (Azure OpenAI / Ollama)
  - **MCP (Model Context Protocol)** para exponer la API como herramientas que el agente consume
  - Agente conversacional que consulta productos, crea registros y ejecuta acciones sobre la API
  - Integración del agente en el front Blazor como asistente

## 📋 Backlog vivo

Los pendientes concretos, detectados en conversación o derivados del roadmap, viven en `BACKLOG.md`. Ese fichero se actualiza en cada sesión de trabajo: lo que se apunta entra, lo que se cierra sale.