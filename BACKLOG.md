## Pendientes técnicos detectados
- [ ] Paginación real de servidor en la API (skip/take) cuando los datos crezcan
- [ ] Sustituir entidad `Product` como payload de wire por un `ProductDto` (reduce acople)
- [ ] Limpieza C10: borrar `NavMenu.razor` y su css huérfanos, css viejos de la plantilla, splash de arranque del index.html
- [ ] Alta conjunta User+Person desde el front (formulario del perforador)
- [x] Validación de cliente con anotaciones + pintado de ProblemDetails del middleware en el front (¡Hecho!)

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
- [ ] Endpoint en backend para mejorar descripción de producto con IA (modelo ligero vía API) + Modal de confirmación en front
- [ ] (Futuro) Agente conversacional sobre la API (requerirá Semantic Kernel)
- [ ] (Futuro) Asistente integrado en el front Blazor

## Calidad y despliegue
- [ ] Tests unitarios (xUnit)
- [ ] Tests de integración con Testcontainers
- [ ] Tests de arquitectura
- [ ] Dockerización completa y CI/CD

## Deuda de front y UX (aparcados del chat)
[x] Modal de confirmación Bootstrap para sustituir window.confirm en borrados (¡Hecho!)
[x] Componente reutilizable NumberInput.razor (input numérico con validación + decimal locale-aware) (¡Hecho!)
[x] Toast de errores auto-ocultables en grids y formularios (¡Hecho!)
[ ] Refactor jerarquía PageBaseComponent / FormBaseComponent (grids vs formularios)
[ ] Unificar estilo de rutas de endpoints (REST sustantivos vs verbos RPC)
[ ] Si la consulta de producto crece, separarla a un componente ProductDetail propio