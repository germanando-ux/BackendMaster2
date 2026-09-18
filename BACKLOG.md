# Backlog BackendMaster2

## Pendientes técnicos detectados
- [ ] Paginación real de servidor en la API (skip/take) cuando los datos crezcan
- [ ] Sustituir entidad `Product` como payload de wire por un `ProductDto` (reduce acople)
- [ ] Limpieza C10: borrar `NavMenu.razor` y su css huérfanos, css viejos de la plantilla, splash de arranque del index.html
- [ ] Alta conjunta User+Person desde el front (formulario del perforador)
- [ ] Validación de cliente con anotaciones + pintado de ProblemDetails del middleware en el front

## Enriquecimiento del dominio
- [ ] Categorías de producto (maestro + relación 1:N)
- [ ] Tallas (maestro + relación N:M)
- [ ] Características (decidir JSONB vs EAV cuando llegue el C8)

## Procesamiento asíncrono y tiempo real
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

## Inteligencia Artificial
- [ ] Semantic Kernel + Microsoft.Extensions.AI + MCP
- [ ] Agente conversacional sobre la API
- [ ] Asistente integrado en el front Blazor

## Calidad y despliegue
- [ ] Tests unitarios (xUnit)
- [ ] Tests de integración con Testcontainers
- [ ] Tests de arquitectura
- [ ] Dockerización completa y CI/CD