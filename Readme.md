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

**Bloque C (Frontend):** 🚧 En progreso
- Blazor WebAssembly standalone
- Bootstrap como única librería de UI (MudBlazor evaluado y retirado)
- Layout propio: sidebar colapsable, navbar y tema de marca
- Página de productos con listado, estados de carga/error y tabla tematizada

## 🛠️ Stack tecnológico

### Backend
- **.NET 8** con ASP.NET Core
- **Entity Framework Core** + PostgreSQL dockerizado
- **FluentValidation** para validación de entrada
- **BCrypt** para hashing de contraseñas
- **JWT Bearer** para autenticación
- **Scalar** para documentación OpenAPI interactiva

### Frontend
- **Blazor WebAssembly** (SPA con C#)
- **Bootstrap** (para componentes HTML personalizados)

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

## 🗺️ Roadmap

### Completado
- ✅ **Bloque A:** Backend completo con autenticación JWT, gestión de productos, middleware de errores
- ✅ **Bloque A.5:** Validación JWT en endpoints protegidos
- ✅ **Configuración:** PostgreSQL dockerizado, EF Core, migraciones, seeds

### En progreso
- 🚧 **Bloque C:** Frontend Blazor WebAssembly
  - ✅ Esqueleto de layout con Bootstrap 
  - ✅ Configuración CORS en backend
  - ⏳ Integración con API de autenticación
  - ⏳ Gestión de estado de sesión
  - ⏳ Páginas de productos y usuarios

### Futuro
- ⏳ **Bloque D:** Procesamiento asíncrono y tareas en segundo plano
  - **BackgroundServices** para tareas periódicas y limpieza automática
  - **Gestor de trabajos** (Hangfire o similar) para colas de trabajo persistentes
  - **Batch processing** para importaciones masivas de productos
  - **SignalR** para notificaciones de progreso en tiempo real
  - **Redis** para caché distribuido y rate limiting
  - **RabbitMQ** para mensajería asíncrona entre módulos
  - **Elasticsearch + Kibana** para búsqueda avanzada de productos y análisis de logs

- ⏳ **Bloque E:** Enriquecimiento del dominio
  - Categorías de producto (maestro nuevo + relación 1:N)
  - Tallas (maestro + relación N:M)
  - Características (JSONB o EAV según necesidad de filtrado)
  - Auditoría de cambios en entidades críticas

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

Los pendientes concretos, detectados en conversación o derivados del roadmap, viven en [`BACKLOG.md`](BACKLOG.md). Ese fichero se actualiza en cada sesión de trabajo: lo que se apunta entra, lo que se cierra sale.

