# 🏗️ Arquitectura del Sistema Excursionistas

## 📐 Diagrama de Arquitectura General

```
┌─────────────────────────────────────────────────────────────────────┐
│                          Cliente / Usuario                           │
│         (Navegador, Postman, curl, Aplicación externa)              │
└────────────────────────────────┬────────────────────────────────────┘
                                 │ HTTP/HTTPS
                                 │ REST API (JSON)
┌────────────────────────────────▼────────────────────────────────────┐
│                        API LAYER (ASP.NET Core)                      │
│  ┌──────────────┐  ┌──────────────┐  ┌────────────────────────┐   │
│  │ Controllers  │  │  Middleware  │  │  Swagger/OpenAPI       │   │
│  │ - Elements   │  │ - Exception  │  │  - Documentation       │   │
│  │ - Optimization│  │ - Logging    │  │  - Interactive UI      │   │
│  └──────┬───────┘  └──────┬───────┘  └────────────────────────┘   │
└─────────┼──────────────────┼──────────────────────────────────────┘
          │                  │
          │ DTOs             │ Error Handling
          │                  │
┌─────────▼──────────────────▼──────────────────────────────────────┐
│                   APPLICATION LAYER (Use Cases)                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────────┐  │
│  │   Services      │  │   Validators    │  │   DTOs           │  │
│  │ - ElementService│  │ - FluentValid.  │  │ - Requests       │  │
│  │ - OptimizSvc   │  │ - Rules         │  │ - Responses      │  │
│  └────────┬────────┘  └─────────────────┘  └──────────────────┘  │
│           │                                                         │
│           │ Interfaces (IElementService, IOptimizationService)     │
└───────────┼─────────────────────────────────────────────────────────┘
            │
            │ Business Logic
            │
┌───────────▼──────────────────────────────────────────────────────┐
│                      DOMAIN LAYER (Core Logic)                    │
│  ┌────────────────┐  ┌──────────────────┐  ┌─────────────────┐  │
│  │   Entities     │  │  Value Objects   │  │   Interfaces    │  │
│  │ - Element      │  │ - OptimizResult  │  │ - IElementRepo  │  │
│  │ - Configuration│  │                  │  │ - IOptimizer    │  │
│  └────────────────┘  └──────────────────┘  └─────────────────┘  │
│  ┌────────────────┐  ┌──────────────────┐                        │
│  │   Exceptions   │  │  Domain Services │                        │
│  │ - DomainExc    │  │ - OptimizerSvc   │                        │
│  │ - InvalidElmt  │  │ - Algorithm Logic│                        │
│  └────────────────┘  └──────────────────┘                        │
└───────────┬──────────────────────────────────────────────────────┘
            │
            │ Persistence Contracts
            │
┌───────────▼──────────────────────────────────────────────────────┐
│              INFRASTRUCTURE LAYER (External Concerns)             │
│  ┌────────────────┐  ┌──────────────────┐  ┌─────────────────┐  │
│  │  Repositories  │  │    DbContext     │  │   Migrations    │  │
│  │ - ElementRepo  │  │ - EF Core        │  │ - Initial       │  │
│  │ - Impl IRepo   │  │ - Configuration  │  │ - Versioning    │  │
│  └────────┬───────┘  └────────┬─────────┘  └─────────────────┘  │
│           │                   │                                   │
│           │                   │                                   │
│           │        ┌──────────▼─────────┐                        │
│           └────────►   Data Seeding     │                        │
│                    │  - DatabaseSeeder  │                        │
│                    └────────────────────┘                        │
└───────────────────────────────┬──────────────────────────────────┘
                                │
                                │ SQL Queries
                                │
┌───────────────────────────────▼──────────────────────────────────┐
│                         DATABASE                                  │
│  ┌──────────────────┐              ┌──────────────────┐          │
│  │  SQLite (Dev)    │      OR      │ PostgreSQL (Prod)│          │
│  │ - File-based     │              │ - Server-based   │          │
│  │ - Embedded       │              │ - Scalable       │          │
│  └──────────────────┘              └──────────────────┘          │
└───────────────────────────────────────────────────────────────────┘
```

---

## 🎯 Principios SOLID Aplicados

### 1. Single Responsibility Principle (SRP)

**✅ Implementado:** Cada clase tiene una única razón para cambiar

#### Ejemplos:
```csharp
// ❌ ANTES (violación de SRP)
public class Element
{
    public void Save() { /* acceso a DB */ }
    public void Validate() { /* lógica de negocio */ }
    public void SendEmail() { /* infraestructura */ }
}

// ✅ DESPUÉS (SRP aplicado)
public class Element // Solo representa la entidad
{
    public int Id { get; set; }
    public string Name { get; set; }
    // ... solo propiedades y lógica de negocio mínima
}

public class ElementRepository : IElementRepository // Solo persistencia
{
    public async Task<Element> SaveAsync(Element element) { }
}

public class ElementValidator : AbstractValidator<CreateElementRequest> // Solo validación
{
    public ElementValidator() 
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
```

### 2. Open/Closed Principle (OCP)

**✅ Implementado:** Abierto para extensión, cerrado para modificación

#### Ejemplo: Algoritmo de Optimización Extensible
```csharp
// Interface permite nuevas estrategias sin modificar código existente
public interface IOptimizerService
{
    OptimizationResult Optimize(/* params */);
}

// Implementación actual: Greedy
public class OptimizerService : IOptimizerService
{
    // Estrategia greedy por eficiencia calórica
}

// Futuro: Agregar estrategia dinámica sin modificar código existente
public class DynamicProgrammingOptimizer : IOptimizerService
{
    // Nueva estrategia sin modificar OptimizerService
}
```

### 3. Liskov Substitution Principle (LSP)

**✅ Implementado:** Subtipos deben ser sustituibles por sus tipos base

#### Ejemplo:
```csharp
// Cualquier IElementRepository puede usarse sin cambiar comportamiento
IElementRepository repository = new ElementRepository(dbContext);
// O en el futuro:
IElementRepository repository = new CachedElementRepository(dbContext, cache);
IElementRepository repository = new ReadOnlyElementRepository(dbContext);

// El servicio no necesita saber qué implementación específica usa
public class ElementService
{
    private readonly IElementRepository _repository; // Acepta cualquier implementación
    
    public ElementService(IElementRepository repository)
    {
        _repository = repository; // LSP: todas las implementaciones funcionan igual
    }
}
```

### 4. Interface Segregation Principle (ISP)

**✅ Implementado:** Interfaces pequeñas y específicas

#### Ejemplo:
```csharp
// ❌ ANTES (violación de ISP)
public interface IRepository
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<IEnumerable<T>> SearchAsync(string query);
    Task<int> CountAsync();
    Task BulkInsertAsync(IEnumerable<T> entities);
}

// ✅ DESPUÉS (ISP aplicado)
public interface IElementRepository // Solo lo necesario
{
    Task<Element?> GetByIdAsync(int id);
    Task<IEnumerable<Element>> GetAllActiveAsync();
    Task<Element?> GetByNameAsync(string name);
    Task<Element> AddAsync(Element element);
    Task UpdateAsync(Element element);
    Task DeleteAsync(int id);
}

public interface IOptimizerService // Interface separada para optimización
{
    OptimizationResult Optimize(/* params */);
}
```

### 5. Dependency Inversion Principle (DIP)

**✅ Implementado:** Depender de abstracciones, no de implementaciones concretas

#### Ejemplo:
```csharp
// ❌ ANTES (violación de DIP)
public class ElementService
{
    private readonly ElementRepository _repository; // Dependencia concreta
    
    public ElementService()
    {
        _repository = new ElementRepository(); // Acoplamiento fuerte
    }
}

// ✅ DESPUÉS (DIP aplicado)
public class ElementService : IElementService
{
    private readonly IElementRepository _repository; // Dependencia de abstracción
    private readonly ILogger<ElementService> _logger;
    
    public ElementService(
        IElementRepository repository, // Inyección de dependencias
        ILogger<ElementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}

// Configuración en Program.cs
services.AddScoped<IElementRepository, ElementRepository>(); // Inversión de control
services.AddScoped<IElementService, ElementService>();
```

---

## 🧱 Capas de la Arquitectura

### 1. API Layer (`Excursionistas.API`)

**Responsabilidad:** Punto de entrada HTTP, manejo de requests/responses

**Componentes:**
```
Excursionistas.API/
├── Controllers/
│   ├── ElementsController.cs        # CRUD de elementos
│   └── OptimizationController.cs    # Cálculo de optimización
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs # Manejo global de errores
├── Extensions/
│   └── ServiceCollectionExtensions.cs # Configuración DI
└── Program.cs                        # Entry point, configuración
```

**Características:**
- RESTful endpoints con verbos HTTP estándar
- Content negotiation (JSON)
- Swagger/OpenAPI para documentación
- CORS configurado
- Exception middleware para respuestas consistentes
- Logging estructurado

**Ejemplo de Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ElementsController : ControllerBase
{
    private readonly IElementService _elementService;
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ElementResponse>), 200)]
    public async Task<ActionResult<IEnumerable<ElementResponse>>> GetAll()
    {
        var elements = await _elementService.GetAllAsync();
        return Ok(elements);
    }
}
```

### 2. Application Layer (`Excursionistas.Application`)

**Responsabilidad:** Casos de uso, orquestación de lógica de negocio

**Componentes:**
```
Excursionistas.Application/
├── DTOs/
│   ├── Request/
│   │   ├── CreateElementRequest.cs
│   │   ├── UpdateElementRequest.cs
│   │   └── CalculateOptimizationRequest.cs
│   └── Response/
│       ├── ElementResponse.cs
│       ├── OptimizationResultResponse.cs
│       └── ErrorResponse.cs
├── Interfaces/
│   ├── IElementService.cs
│   └── IOptimizationService.cs
├── Services/
│   ├── ElementService.cs            # CRUD business logic
│   └── OptimizationService.cs       # Orchestrates optimization
├── Validators/
│   ├── CreateElementRequestValidator.cs
│   ├── UpdateElementRequestValidator.cs
│   └── CalculateOptimizationRequestValidator.cs
└── Mappings/
    └── MappingProfile.cs             # AutoMapper configuration
```

**Patrón de Servicio:**
```csharp
public class ElementService : IElementService
{
    private readonly IElementRepository _repository;
    private readonly IMapper _mapper;
    
    public async Task<ElementResponse> CreateAsync(CreateElementRequest request)
    {
        // 1. Validación (ya hecha por FluentValidation)
        // 2. Verificar duplicados
        var existing = await _repository.GetByNameAsync(request.Name);
        if (existing != null)
            throw InvalidElementException.NameAlreadyExists(request.Name);
        
        // 3. Mapear DTO a Entity
        var element = _mapper.Map<Element>(request);
        
        // 4. Persistir
        var saved = await _repository.AddAsync(element);
        
        // 5. Retornar DTO de respuesta
        return _mapper.Map<ElementResponse>(saved);
    }
}
```

### 3. Domain Layer (`Excursionistas.Domain`)

**Responsabilidad:** Lógica de negocio pura, reglas de dominio

**Componentes:**
```
Excursionistas.Domain/
├── Entities/
│   ├── Element.cs                   # Entidad principal
│   └── Configuration.cs             # Configuración de optimización
├── ValueObjects/
│   └── OptimizationResult.cs        # Resultado inmutable
├── Exceptions/
│   ├── DomainException.cs           # Base exception
│   ├── InvalidElementException.cs
│   ├── InvalidConfigurationException.cs
│   └── NoSolutionFoundException.cs
├── Interfaces/
│   ├── IElementRepository.cs        # Contrato de repositorio
│   └── IOptimizerService.cs         # Contrato de optimización
└── Services/
    └── OptimizerService.cs          # Algoritmo greedy
```

**Características del Dominio:**
- Sin dependencias externas (puro C#)
- Entidades con encapsulación
- Validaciones de reglas de negocio
- Value Objects inmutables
- Factory methods para creación

**Ejemplo de Entidad:**
```csharp
public class Element
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public double Calories { get; set; }
    
    // Propiedades calculadas (solo getter)
    public double CalorieEfficiency => Calories > 0 && Weight > 0 
        ? Calories / Weight 
        : 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Método de dominio
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Algoritmo de Optimización (Greedy):**
```csharp
public class OptimizerService : IOptimizerService
{
    public OptimizationResult Optimize(
        List<Element> elements,
        double minimumCalories,
        double maximumWeight)
    {
        // 1. Ordenar por eficiencia calórica descendente
        var sortedElements = elements
            .Where(e => e.Calories > 0 && e.Weight > 0)
            .OrderByDescending(e => e.CalorieEfficiency)
            .ToList();
        
        var selectedElements = new List<Element>();
        double totalWeight = 0;
        double totalCalories = 0;
        
        // 2. Greedy: tomar elementos con mejor eficiencia
        foreach (var element in sortedElements)
        {
            if (totalWeight + element.Weight <= maximumWeight)
            {
                selectedElements.Add(element);
                totalWeight += element.Weight;
                totalCalories += element.Calories;
                
                // 3. Verificar si ya cumplimos el mínimo
                if (totalCalories >= minimumCalories)
                    break;
            }
        }
        
        // 4. Verificar solución viable
        if (totalCalories < minimumCalories)
            throw NoSolutionFoundException.InsufficientCalories(
                minimumCalories, totalCalories);
        
        return OptimizationResult.CreateSuccess(
            selectedElements, totalWeight, totalCalories);
    }
}
```

### 4. Infrastructure Layer (`Excursionistas.Infrastructure`)

**Responsabilidad:** Acceso a datos, servicios externos

**Componentes:**
```
Excursionistas.Infrastructure/
├── Data/
│   ├── ExcursionistasDbContext.cs   # EF Core DbContext
│   ├── Migrations/
│   │   └── 20260308185025_InitialCreate.cs
│   ├── Seeds/
│   │   └── DatabaseSeeder.cs        # Data seeding
│   └── Configurations/
│       ├── ElementConfiguration.cs   # Fluent API config
│       └── ConfigurationConfiguration.cs
└── Repositories/
    └── ElementRepository.cs          # Implementación IElementRepository
```

**DbContext:**
```csharp
public class ExcursionistasDbContext : DbContext
{
    public DbSet<Element> Elements => Set<Element>();
    public DbSet<Configuration> Configurations => Set<Configuration>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración fluent
        modelBuilder.Entity<Element>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Weight).IsRequired();
            entity.Property(e => e.Calories).IsRequired();
        });
    }
}
```

**Repository Implementation:**
```csharp
public class ElementRepository : IElementRepository
{
    private readonly ExcursionistasDbContext _context;
    
    public async Task<IEnumerable<Element>> GetAllActiveAsync()
    {
        return await _context.Elements
            .Where(e => e.IsActive)
            .AsNoTracking() // Optimización para lectura
            .ToListAsync();
    }
    
    public async Task<Element> AddAsync(Element element)
    {
        _context.Elements.Add(element);
        await _context.SaveChangesAsync();
        return element;
    }
}
```

---

## 🔄 Flujo de Datos Completo

### Ejemplo: Crear un Elemento

```
1. REQUEST
   ↓
POST /api/elements
{
  "name": "Gel Energético",
  "weight": 0.05,
  "calories": 120
}
   ↓
2. CONTROLLER (ElementsController)
   - Recibe CreateElementRequest
   - FluentValidation automática
   ↓
3. APPLICATION SERVICE (ElementService)
   - Verifica nombre duplicado
   - Mapea Request → Entity
   ↓
4. DOMAIN (Element entity)
   - Se crea instancia
   - Calcula CalorieEfficiency
   ↓
5. INFRASTRUCTURE (ElementRepository)
   - Persiste en base de datos
   - EF Core genera SQL
   ↓
6. DATABASE (SQLite/PostgreSQL)
   - INSERT INTO Elements...
   ↓
7. RESPONSE
   ← ElementResponse {id: 21, ...}
   ← HTTP 201 Created
```

### Ejemplo: Calcular Optimización

```
1. REQUEST
   ↓
POST /api/optimization/calculate
{
  "minimumCalories": 1500,
  "maximumWeight": 5,
  "elementIds": []
}
   ↓
2. CONTROLLER (OptimizationController)
   - Recibe CalculateOptimizationRequest
   - Validación FluentValidation
   ↓
3. APPLICATION SERVICE (OptimizationService)
   - Obtiene elementos (repository)
   - Filtra por IDs si se especifican
   ↓
4. DOMAIN SERVICE (OptimizerService)
   - Aplica algoritmo greedy
   - Ordena por eficiencia
   - Selecciona elementos
   ↓
5. DOMAIN (OptimizationResult)
   - Value object inmutable
   - Contiene resultado
   ↓
6. APPLICATION SERVICE
   - Mapea a OptimizationResultResponse
   ↓
7. RESPONSE
   ← OptimizationResultResponse
   ← HTTP 200 OK
```

---

## 🧪 Testing Strategy

### Pirámide de Tests

```
           ╱╲
          ╱  ╲
         ╱ E2E ╲         2 tests
        ╱────────╲
       ╱          ╲
      ╱ Integration ╲    8 tests
     ╱──────────────╲
    ╱                ╲
   ╱   Unit Tests     ╲  14 tests
  ╱────────────────────╲
```

### Capas Testeadas

**Unit Tests:**
- Domain entities
- Validators
- Services (con mocks)

**Integration Tests:**
- Controllers con DB en memoria
- Flujo completo sin mocks

**Ejemplo de Test:**
```csharp
[Fact]
public async Task Calculate_WithValidRequest_ReturnsOptimalSolution()
{
    // Arrange
    var request = new CalculateOptimizationRequest
    {
        MinimumCalories = 1500,
        MaximumWeight = 5,
        ElementIds = new List<int>()
    };
    
    // Act
    var result = await _optimizationService.CalculateOptimizationAsync(request);
    
    // Assert
    result.Success.Should().BeTrue();
    result.TotalCalories.Should().BeGreaterThanOrEqualTo(1500);
    result.TotalWeight.Should().BeLessThanOrEqualTo(5);
}
```

---

## 📊 Patrones de Diseño Utilizados

| Patrón | Ubicación | Propósito |
|--------|-----------|-----------|
| Repository | Infrastructure | Abstracción de acceso a datos |
| Service Layer | Application | Lógica de negocio desacoplada |
| Factory Method | Domain | Creación de Value Objects |
| Strategy | Domain | Algoritmo de optimización intercambiable |
| Dependency Injection | API | Inversión de control |
| DTO | Application | Transfer de datos entre capas |
| Unit of Work | Infrastructure | EF Core DbContext |
| Middleware | API | Pipeline de procesamiento HTTP |

---

## 🔐 Seguridad y Mejores Prácticas

### Implementado:
- ✅ SQL Injection: Prevenido por EF Core parameterized queries
- ✅ Validación de entrada: FluentValidation en todos los endpoints
- ✅ Exception handling: Middleware global, no expone stack traces
- ✅ Logging: Información sensible no loggeada
- ✅ CORS: Configurado con origins específicos

### Pendiente (Futuro):
- ⏳ Autenticación JWT
- ⏳ Rate limiting
- ⏳ HTTPS obligatorio en producción
- ⏳ API keys para clientes externos

---

**Documento creado:** 8 de Marzo, 2026  
**Versión del Sistema:** 1.0.0  
**Autor:** Anderson Lozano
