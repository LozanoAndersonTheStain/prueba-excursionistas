# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- **Swagger/OpenAPI Integration**: 
  - Habilitado Swagger UI en modo Development
  - Swashbuckle.AspNetCore 6.5.0 compatible con .NET 8.0
  - Generación automática de documentación XML
  - UI interactiva en ruta raíz (http://localhost:5000)

### Changed
- **Package Updates**:
  - Swashbuckle.AspNetCore: 10.1.4 → 6.5.0 (compatible con .NET 8.0)
  - Habilitada generación de archivo de documentación XML en .csproj

### Documentation
- ⭐ **[SWAGGER_GUIDE.md](SWAGGER_GUIDE.md)**: Guía completa de uso de Swagger UI (450+ líneas)
- ⭐ **[docs/REQUIREMENTS_VERIFICATION.md](docs/REQUIREMENTS_VERIFICATION.md)**: Matriz de cumplimiento de requisitos (580+ líneas) - ✅ 100% cumplimiento
- ⭐ **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)**: Documentación completa de arquitectura (850+ líneas) - SOLID, patrones, diagramas
- ⭐ **[docs/POSTGRESQL_MIGRATION.md](docs/POSTGRESQL_MIGRATION.md)**: Guía completa de migración a PostgreSQL (800+ líneas) - Instalación, scripts, troubleshooting
- ⭐ **[docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)**: Guía completa de deployment (1000+ líneas) - Azure, AWS, Docker, CI/CD
- **README.md actualizado**: PostgreSQL completo, Docker Compose, Swagger, tabla de estado, índice de documentación

### Fixed
- ✅ **Swagger**: Resuelto incompatibilidad con .NET 8.0

## [1.0.0] - 2026-03-08

### 🎉 Initial Release

#### Added

##### Core Features
- **Knapsack Optimization Algorithm**: Implementación completa del algoritmo de optimización tipo knapsack (mochila) para seleccionar elementos que cumplen con calorías mínimas mientras minimizan el peso total
- **Clean Architecture**: Estructura de proyecto con separación clara de capas (Domain, Application, Infrastructure, API)
- **SOLID Principles**: Aplicación consistente de principios SOLID en todo el código

##### Domain Layer
- **Entities**: 
  - `Element`: Entidad que representa elementos de equipo con propiedades de peso, calorías y eficiencia calórica calculada
  - `Configuration`: Entidad para definir restricciones de optimización (calorías mínimas, peso máximo)
- **Value Objects**: 
  - `OptimizationResult`: Resultado inmutable de optimización con factory methods
- **Custom Exceptions**: 
  - `DomainException`: Excepción base con código de error
  - `InvalidElementException`: Validaciones de elementos
  - `InvalidConfigurationException`: Validaciones de configuración
  - `NoSolutionFoundException`: Manejo de casos sin solución viable
- **Interfaces**: 
  - `IElementRepository`: Contrato de repositorio con operaciones async
  - `IOptimizerService`: Contrato del servicio de optimización

##### Application Layer
- **DTOs de Request**:
  - `CreateElementRequest`: Crear nuevos elementos
  - `UpdateElementRequest`: Actualizar elementos existentes
  - `CalculateOptimizationRequest`: Solicitar cálculo de optimización
- **DTOs de Response**:
  - `ElementResponse`: Respuesta con datos de elemento
  - `OptimizationResultResponse`: Resultado de optimización con estadísticas
  - `ErrorResponse`: Respuestas de error estandarizadas
- **Services**: 
  - `ElementService`: Lógica de negocio para gestión de elementos
  - `OptimizationService`: Implementación del algoritmo de optimización
- **Validators**: FluentValidation para todos los DTOs de request
- **AutoMapper**: Configuración de mapeo entre entidades y DTOs

##### Infrastructure Layer
- **Entity Framework Core 10.0**:
  - Soporte para SQLite (desarrollo) y PostgreSQL (producción)
  - Configuración fluent para entidades
  - Migraciones automáticas al inicio
  - Data seeding opcional con 20 elementos de ejemplo
- **Repository Pattern**: Implementación concreta de `IElementRepository`
- **DbContext**: `ExcursionistasDbContext` con configuración avanzada
- **Design-time Factory**: Para generación de migraciones

##### API Layer
- **RESTful Controllers**:
  - `ElementsController`: CRUD completo para elementos
    - GET /api/elements - Listar todos
    - GET /api/elements/{id} - Obtener por ID
    - POST /api/elements - Crear elemento
    - PUT /api/elements/{id} - Actualizar elemento
    - DELETE /api/elements/{id} - Soft delete
  - `OptimizationController`: Cálculo de optimización
    - POST /api/optimization/calculate - Calcular combinación óptima
- **Exception Handling Middleware**: Manejo global de excepciones con respuestas consistentes
- **CORS Configuration**: Configuración flexible vía variables de entorno
- **Logging**: Logging estructurado con diferentes niveles
- **Environment Variables**: Integración con DotNetEnv para configuración

##### Testing
- **Unit Tests** (xUnit + Moq + FluentAssertions):
  - Tests de entidades del dominio
  - Tests de validadores
  - Tests de servicios con mocks
- **Integration Tests**:
  - Tests end-to-end de controllers
  - Tests con base de datos en memoria
  - 24 pruebas en total, todas pasando ✅

##### DevOps & Infrastructure
- **Docker Support**:
  - Dockerfile multi-stage para optimizar tamaño de imagen
  - docker-compose.yml para orquestación
  - Volúmenes para persistencia de datos y logs
- **Environment Configuration**:
  - `.env.example` con todas las variables documentadas
  - Soporte para múltiples proveedores de base de datos
  - Configuración flexible de CORS y puertos

##### Documentation
- **README.md completo**:
  - Descripción del problema y solución
  - Ejemplos de uso con curl
  - Guía de instalación para desarrollo y producción
  - Documentación de endpoints con ejemplos reales de request/response
  - Instrucciones de testing y Docker
- **ESCALABILIDAD.md**: Estrategias de escalado y mejoras futuras
- **CHANGELOG.md**: Este archivo
- **XML Documentation**: Comentarios XML en español en todo el código público

##### Best Practices
- **Internationalization**: Código en inglés, comentarios en español
- **Soft Delete**: Los elementos se marcan como inactivos en lugar de eliminarse
- **Immutability**: Value objects inmutables con factory methods
- **Dependency Injection**: DI configurado en todos los servicios
- **Async/Await**: Operaciones asíncronas en toda la aplicación
- **Repository Pattern**: Abstracción de acceso a datos
- **Factory Pattern**: Creación controlada de objetos complejos

#### Technical Stack
- .NET 10.0
- Entity Framework Core 10.0.0
- AutoMapper 12.0.1
- FluentValidation 11.11.0
- xUnit 2.9.3
- Moq 4.20.72
- FluentAssertions 7.0.0
- DotNetEnv 3.1.1
- SQLite (desarrollo)
- PostgreSQL 16+ (producción)
- Docker & Docker Compose

#### Known Issues
- ~~Swagger/OpenAPI temporalmente deshabilitado por compatibilidad con .NET 10.0~~ ✅ **RESUELTO** - Swagger habilitado con Swashbuckle 6.5.0 para .NET 8.0

#### Performance
- **Optimization Algorithm**: Tiempo de ejecución O(n*w) donde n = número de elementos, w = peso máximo
- **Database Queries**: Optimizadas con AsNoTracking para lecturas
- **Test Execution**: 24 tests ejecutándose en ~3.3 segundos

---

## [Unreleased]

### Added
- **Swagger/OpenAPI Do en Docker (actualmente solo disponible en Development local)loración interactiva de la API
  - Documentación XML generada automáticamente desde comentarios del código
  - Interfaz disponible en `http://localhost:5000` en modo Development
  - Metadata completa de endpoints, modelos y códigos de respuesta
  - Ejemplos de request/response para todos los endpoints

### Changed
- **Swashbuckle.AspNetCore**: Actualizado a versión 6.5.0 (compatible con .NET 8.0)
- **XML Documentation**: Habilitada generación de archivo XML para documentación

### Planned
- Re-habilitar Swagger/OpenAPI con versión compatible con .NET 10.0
- Caché distribuido (Redis) para resultados de optimización
- Rate limiting para API pública
- Health checks y métricas de observabilidad
- CI/CD pipeline con GitHub Actions
- Cobertura de código con reportes

---

**Desarrollado con ❤️ por Anderson Lozano**
