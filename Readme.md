# BackendMaster2

Proyecto de aprendizaje progresivo para consolidar conocimientos de .NET, arquitectura modular y desarrollo full-stack con Blazor WebAssembly.

## 🎯 Objetivo

Construir una aplicación completa (backend + frontend) que sirva como portfolio técnico, explorando patrones de arquitectura modernos mientras se aprenden conceptos nuevos paso a paso.

## 📊 Estado actual

**Bloque A (Backend):** ✅ Completado
- API REST con ASP.NET Core 10
- Autenticación JWT con refresh tokens
- PostgreSQL dockerizado + EF Core
- Middleware de excepciones con ProblemDetails
- Validación con FluentValidation
- Scalar/OpenAPI para documentación interactiva
- CORS configurado para desarrollo
- ✅ Búsqueda híbrida semántica con `pgvector` e IA (extracción de intención + umbral de similitud)

**Bloque C (Frontend):** 🚧 En progreso
- Blazor WebAssembly standalone
- Bootstrap como única librería de UI (MudBlazor evaluado y retirado)
- Layout propio: sidebar colapsable, navbar y tema de marca
- Página de productos con listado, estados de carga/error y tabla tematizada
- CRUD de maestros: colores, fuentes de datos y marketplaces
- Login con validación de cliente (anotaciones) y pintado de ProblemDetails del middleware

**Enriquecimiento del dominio (avance de Bloque E):** ✅
- Maestros `DataSource` y `Marketplace` con CRUD completo (backend + frontend), validaciones y seeds
- Categorías jerárquicas (`Category` con `ParentId`) y relación N:M con productos (`ProductCategory` con `IsPrimary`) vía migración EF Core
- Endpoint de mejora de descripción de producto con IA + modal de confirmación y toast en el front

## 🛠️ Stack tecnológico

### Backend
- **.NET 10** con ASP.NET Core
- **Entity Framework Core** + PostgreSQL dockerizado
- **Npgsql + Pgvector**: Búsqueda vectorial y operadores de distancia de coseno (`<=>`)
- **Microsoft.Extensions.AI**: Extracción de intención de búsqueda en lenguaje natural y generación de embeddings
- **FluentValidation** para validación de entrada
- **BCrypt** para hashing de contraseñas
- **JWT Bearer** para autenticación
- **Scalar** para documentación OpenAPI interactiva

### Frontend
- **Blazor WebAssembly** (SPA con C#)
- **Bootstrap** (para componentes HTML personalizados)

## 📁 Estructura del proyecto

```
BackendMaster2.slnx
├── BackendMaster2.Api          # Host ASP.NET Core: controllers, middlewares, SignalR, settings
├── BackendMaster2.Modules      # Dominio modular: Auth + Catálogo (DbContext, entidades, servicios, repositorios, migraciones)
│   ├── Auth                    # Módulo de autenticación (JWT + refresh tokens)
│   ├── Data                    # AppDbContext
│   ├── Dtos                    # Contratos de la API
│   ├── Mappings                # Mapeo entidad <-> DTO
│   ├── Migrations              # Migraciones EF Core (incluye pgvector)
│   └── Services                # ProductService, CatalogService, MasterDataService, AiService
├── BackendMaster2.Shared       # Utilidades transversales (VectorExtensions, validaciones de tipos)
└── BackendMaster2.Web          # Frontend Blazor WebAssembly (páginas, layout, servicios HTTP, DTOs espejo)
```

## 🚀 Cómo ejecutar el proyecto

### Requisitos
- SDK **.NET 10**
- **Docker** (para PostgreSQL + pgvector)

### 1. Levantar la base de datos

```bash
docker compose up -d
```

Esto arranca `pgvector/pgvector:pg17` en el puerto `5432` (base `backendmaster2`, usuario `postgres`). Las credenciales están en `compose.yaml`.

### 2. Ejecutar la API

```bash
dotnet run --project BackendMaster2.Api
```

Servida en `http://localhost:5150`. Documentación interactiva (Scalar/OpenAPI) en `http://localhost:5150/scalar`.

### 3. Aplicar migraciones y seeds

Aplica las migraciones de EF Core del proyecto `BackendMaster2.Modules` (por ejemplo con `dotnet ef database update --project BackendMaster2.Modules --startup-project BackendMaster2.Api`) y, una vez arrancada la API, carga los datos iniciales invocando el endpoint `POST /api/seed` (idempotente: genera maestros, usuarios y 200+ productos mock con embeddings).

### 4. Ejecutar el frontend

```bash
dotnet run --project BackendMaster2.Web
```

El front consume la API vía HTTP; CORS ya está configurado para desarrollo.

## 🏗️ Decisiones de arquitectura

### 1. DTOs en cliente y servidor (estrategia de espejo manual)

**Decisión:** Los DTOs existen tanto en el backend (`BackendMaster2.Api.Models.Dtos`) como en el frontend (`BackendMaster2.Web.Models.Dtos`), copiados manualmente.

**Alternativas consideradas:**
- DTOs compartidos en proyecto `Shared` (referencia de proyecto)
- Generación automática con NSwag/OpenAPI
- DTOs solo en backend con tipos anónimos en frontend

**Por qué esta elección:**
- El frontend vive en la misma solución pero es una isla independiente
- Evita acoplar el Web al proyecto Api (no arrastrar dependencias del servidor)
- Permite segregar el frontend a otro repositorio en el futuro sin romper nada
- El coste de mantener los espejos es asumible: el proceso es manual pero controlado, y crece de forma lineal y predecible con el número de contratos

**Consecuencia:** Cada cambio en el contrato requiere actualizar ambos lados manualmente. Para proyectos más grandes, se migraría a generación automática con NSwag.

### 2. Uso flexible de entidades vs DTOs

**Decisión:** En algunos endpoints simples se devuelven entidades directamente, mientras que en endpoints complejos se usan DTOs específicos.

**Alternativa canónica:** Siempre usar DTOs, nunca exponer entidades de dominio.

**Por qué esta flexibilidad:**
- Para endpoints de lectura simple (lista de productos), crear un DTO idéntico a la entidad añade fricción sin beneficio claro
- Los DTOs se usan cuando hay transformación de datos, ocultación de campos internos, o composición de múltiples fuentes

**Regla práctica:** Si el DTO sería idéntico a la entidad y no hay lógica de transformación, se puede devolver la entidad. Si hay que filtrar campos, calcular valores, o combinar datos, se crea un DTO.

### 3. Controllers REST pragmáticos (no estrictamente REST)

**Decisión:** Los controllers siguen convenciones REST en general, pero priorizan claridad y simplicidad sobre adherencia estricta al patrón REST.

**Ejemplos de pragmatismo:**
- Endpoints con verbos POST para acciones que no son creación (`POST /api/auth/refresh`, `POST /api/auth/revoke`)
- URLs con verbos en lugar de sustantivos cuando la acción es más clara que el recurso

**Por qué esta flexibilidad:**
- Para peticiones complejas, REST puro se queda corto y no siempre encaja bien con acciones específicas del dominio
- Reduce la carga cognitiva al navegar el código

### 4. Blazor WebAssembly con Bootstrap puro (MudBlazor evaluado y retirado)

**Decisión:** El frontend usa Bootstrap + HTML puro como única librería de UI.

**Historia:** El proyecto arrancó con MudBlazor como librería principal. Durante el Bloque C se sustituyó progresivamente por Bootstrap y, cuando ningún componente la usaba ya, se retiró por completo en cuatro pasos: links del index.html, usings, registro de servicios y paquete NuGet.

**Por qué Bootstrap puro:**
- Control total: cada clase y cada regla css del proyecto es nuestra y se puede leer
- Menos magia: cualquier comportamiento se depura en el markup, no dentro de una librería
- Aprendizaje más profundo de HTML/CSS y de los mecanismos de Blazor sin capas intermedias
- Cero dependencias de UI externas que versionar

**Coste asumido:** Los componentes ricos (tablas con ordenación, diálogos, toasts) se construyen a mano sobre clases de Bootstrap. Más markup propio, a cambio de entender cada línea.

**Consecuencia:** El sistema de tema vive centralizado en `wwwroot/css/app.css` (token `--color-marca`, override parcial de variables de Bootstrap y extensiones puntuales como `.table-encabezado-marca`).

### 5. Autenticación JWT

JWT con access tokens cortos (15 min) y refresh tokens en base de datos (7 días) con capacidad de revocación. Patrón estándar de la industria.

### 6. Arquitectura modular en el backend

**Decisión:** El código está organizado en módulos (`Auth`, `ProductManagement`) dentro de `BackendMaster2.Modules`, no en carpetas por capa técnica.

**Por qué esta estructura:**
- Cada módulo es autocontenido: tiene sus propios repositorios, servicios, DTOs y entidades
- Facilita la extracción futura de módulos a microservicios si fuera necesario
- Reduce el acoplamiento entre dominios diferentes
- Más fácil de navegar: todo lo relacionado con "productos" está junto, no disperso en Controllers/Services/Repositories

### 7. Búsqueda Híbrida y Umbral de Similitud

**Decisión:** La búsqueda combina filtros duros (precio, stock) en la base de datos con ordenación semántica (`pgvector`), aplicando un umbral de similitud (ej. 60%) en memoria para descartar resultados irrelevantes ("ruido").

**Por qué esta elección:**
- Evita devolver "lo menos malo" cuando no hay coincidencias reales en el dataset.
- El cálculo del `SimilarityScore` se delega a una extensión C# pura (`VectorExtensions.CalculateCosineSimilarity` en `Shared`) para evitar errores de traducción de EF Core y mantener el código limpio, testeable y reutilizable.
- Los DTOs de respuesta de búsqueda (`ProductSearchHybridDto`) están aislados en un archivo de soporte (`SupportDtos.cs`) para no ensuciar los DTOs de dominio principales con campos *ad-hoc*.

## 🗺️ Roadmap

### Completado
- ✅ **Bloque A:** Backend completo con autenticación JWT, gestión de productos, middleware de errores y búsqueda semántica
- ✅ **Bloque A.5:** Validación JWT en endpoints protegidos
- ✅ **Configuración:** PostgreSQL dockerizado, EF Core, migraciones, seeds

### En progreso
- 🚧 **Bloque C:** Frontend Blazor WebAssembly
  - ✅ Esqueleto de layout con Bootstrap
  - ✅ Configuración CORS en backend
  - ✅ Página de productos (listado, formulario, estados de carga/error)
  - ✅ CRUD de maestros en el front: colores, fuentes de datos y marketplaces
  - ✅ Login con validación de cliente y pintado de ProblemDetails
  - ⏳ Gestión de estado de sesión (refresh tokens en el front)
  - ⏳ Integración de categorías en la página de productos

### Futuro
- ⏳ **Bloque D:** Procesamiento asíncrono y tareas en segundo plano
  - **BackgroundServices** para tareas periódicas y limpieza automática
  - **Gestor de trabajos** (Hangfire o similar) para colas de trabajo persistentes
  - **Batch processing** para importaciones masivas de productos
  - 🚧 **SignalR** para notificaciones de progreso en tiempo real (servicio base ya integrado en el Api)
  - **Redis** para caché distribuido y rate limiting
  - **RabbitMQ** para mensajería asíncrona entre módulos
  - **Elasticsearch + Kibana** para búsqueda avanzada de productos y análisis de logs

- 🚧 **Bloque E:** Enriquecimiento del dominio
  - ✅ Maestros `DataSource` y `Marketplace` con CRUD completo y seeds
  - ✅ Categorías jerárquicas (`Category` con `ParentId`) + relación N:M con productos (`ProductCategory.IsPrimary`)
  - ⏳ Tallas (maestro + relación N:M con stock por talla)
  - ⏳ Características (JSONB o EAV según necesidad de filtrado)
  - ⏳ Auditoría de cambios en entidades críticas

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