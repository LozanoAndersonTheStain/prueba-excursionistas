# 📈 Escalabilidad del Sistema Excursionistas

## Visión General
Este documento describe las estrategias y consideraciones para escalar el sistema de optimización de elementos para escalada.

## 🏗️ Arquitectura Actual
- **Tipo**: Monolito modular con arquitectura limpia
- **Base de datos**: SQLite (desarrollo), SQL Server/PostgreSQL (producción)
- **Hosting**: Compatible con Windows, Linux, macOS

## 🚀 Estrategias de Escalabilidad

### 1. Escalado Vertical (Scale Up)
**Descripción**: Aumentar recursos del servidor actual

**Ventajas**:
- Implementación simple
- Sin cambios en código
- Ideal para carga moderada

**Límites**:
- Costo por hardware
- Punto único de falla
- Límite físico del hardware

### 2. Escalado Horizontal (Scale Out)
**Descripción**: Múltiples instancias de la API detrás de un balanceador

```
                      Load Balancer (Nginx/HAProxy)
                             |
              +---------------+---------------+
              |               |               |
         API Instance 1  API Instance 2  API Instance 3
              |               |               |
              +---------------+---------------+
                             |
                    Shared Database (PostgreSQL)
```

**Implementación**:
```yaml
# docker-compose.yml para multi-instancia
version: '3.8'
services:
  api-1:
    build: .
    environment:
      - INSTANCE_ID=1
  api-2:
    build: .
    environment:
      - INSTANCE_ID=2
  api-3:
    build: .
    environment:
      - INSTANCE_ID=3
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
```

### 3. Caché Distribuido
**Tecnologías**: Redis, Memcached

**Casos de uso**:
- Resultados de optimizaciones frecuentes
- Listados de elementos
- Configuraciones del sistema

**Implementación**:
```csharp
// Agregar en Infrastructure
public class CachedElementoRepository : IElementoRepository
{
    private readonly IElementoRepository _inner;
    private readonly IDistributedCache _cache;
    
    public async Task<List<Elemento>> GetAllAsync()
    {
        var cacheKey = "elementos:all";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<List<Elemento>>(cached);
            
        var elementos = await _inner.GetAllAsync();
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(elementos),
            new DistributedCacheEntryOptions 
            { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
            });
            
        return elementos;
    }
}
```

### 4. Base de Datos

#### Réplicas de Lectura
```
            API Instances
                 |
        +--------+--------+
        |                 |
    Write DB          Read Replicas
    (Master)         (Slaves x3)
```

**Configuración**:
```csharp
services.AddDbContext<AppDbContext>(options => 
{
    var connectionString = readOnly 
        ? Configuration["ConnectionStrings:ReadReplica"]
        : Configuration["ConnectionStrings:Master"];
    options.UseNpgsql(connectionString);
});
```

#### Particionamiento (Sharding)
- Por región geográfica
- Por rango de IDs
- Por hash de ID

### 5. Procesamiento Asíncrono
**Para optimizaciones complejas o batch**

```csharp
// Usar colas de mensajes: RabbitMQ, Azure Service Bus
public class OptimizacionBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var mensaje in _cola.ReadAllAsync(stoppingToken))
        {
            var resultado = await _optimizador.CalcularAsync(mensaje);
            await _notificador.NotificarAsync(mensaje.UsuarioId, resultado);
        }
    }
}
```

### 6. Microservicios (Futuro)

**Separación propuesta**:
```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  Elementos API  │────▶│ Optimización API │────▶│  Notificación   │
│   (CRUD básico) │     │  (Algoritmo ML)  │     │     Service     │
└─────────────────┘     └──────────────────┘     └─────────────────┘
```

**Ventajas**:
- Escalado independiente
- Tecnologías específicas por servicio
- Despliegue independiente
- Resiliencia mejorada

### 7. CDN y Assets Estáticos
- Swagger UI
- Documentación
- Assets de frontend (si aplica)

**Proveedores**: Cloudflare, Azure CDN, AWS CloudFront

### 8. Observabilidad

#### Métricas Clave (APM)
```csharp
// Application Insights / Prometheus
services.AddApplicationInsightsTelemetry();

public class OptimizacionService
{
    private readonly ILogger<OptimizacionService> _logger;
    private readonly Meter _meter;
    
    public OptimizacionService()
    {
        _meter = new Meter("Excursionistas.Optimizacion");
        var counter = _meter.CreateCounter<int>("optimizaciones_calculadas");
    }
}
```

**Métricas a monitorear**:
- Requests por segundo (RPS)
- Latencia (p50, p95, p99)
- Tasa de errores
- Uso de CPU/Memoria
- Tamaño de pool de conexiones DB

#### Logging Centralizado
- **Stack ELK**: Elasticsearch, Logstash, Kibana
- **Alternativas**: Seq, Splunk, Azure Log Analytics

### 9. Rate Limiting
```csharp
// Prevenir abuso de API
services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

### 10. Health Checks
```csharp
services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddRedis(Configuration["Redis:ConnectionString"])
    .AddUrlGroup(new Uri("https://external-service.com"), "External Service");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

## 📊 Benchmarks Esperados

| Configuración | RPS | Latencia (p95) | Usuarios Concurrentes |
|--------------|-----|----------------|----------------------|
| Single Instance | 1,000 | 50ms | 100 |
| 3 Instances + LB | 3,000 | 45ms | 300 |
| + Redis Cache | 8,000 | 20ms | 800 |
| + Read Replicas | 15,000 | 25ms | 1,500 |

## 🔒 Consideraciones de Seguridad en Escalado
- HTTPS en todos los puntos
- Secrets en Key Vault (Azure/AWS)
- Red privada entre servicios
- Firewall de aplicaciones web (WAF)
- Autenticación JWT con refresh tokens

## 💰 Estimación de Costos (Azure)

| Componente | Configuración | Costo/mes (USD) |
|-----------|--------------|-----------------|
| App Service | B1 (1 instancia) | $13 |
| App Service | P1v3 (3 instancias) | $310 |
| Azure SQL | Basic | $5 |
| Redis Cache | Basic 1GB | $17 |
| Application Insights | 5GB datos | $12 |
| **Total Mínimo** | | **~$47** |
| **Total Escalado** | | **~$354** |

## 📋 Checklist de Implementación

- [ ] Migrar de SQLite a PostgreSQL/SQL Server
- [ ] Implementar Health Checks
- [ ] Configurar Application Insights
- [ ] Agregar Redis para caché
- [ ] Configurar CI/CD (GitHub Actions/Azure DevOps)
- [ ] Crear Dockerfile optimizado
- [ ] Configurar Kubernetes (opcional)
- [ ] Implementar rate limiting
- [ ] Configurar alertas de monitoreo
- [ ] Documentar runbooks de incidentes

## 🎯 Recomendaciones por Etapa

### Etapa 1: MVP (0-1K usuarios)
- Single instance
- SQLite o Azure SQL Basic
- Logging básico
- **Costo**: ~$20/mes

### Etapa 2: Crecimiento (1K-10K usuarios)
- 2-3 instancias con LB
- PostgreSQL con conexiones pooling
- Redis básico
- Application Insights
- **Costo**: ~$150/mes

### Etapa 3: Escala (10K-100K usuarios)
- Auto-scaling (3-10 instancias)
- PostgreSQL Premium + Read replicas
- Redis Cluster
- CDN para assets
- **Costo**: ~$800/mes

### Etapa 4: Enterprise (100K+ usuarios)
- Kubernetes (AKS/EKS)
- Microservicios
- Multi-región
- DDoS protection
- **Costo**: $3,000+/mes

## 📚 Referencias
- [Microsoft: Arquitectura de aplicaciones web](https://docs.microsoft.com/azure/architecture/)
- [Twelve-Factor App](https://12factor.net/)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---
**Última actualización**: Marzo 2026
