using DotNetEnv;
using Excursionistas.API.Extensions;
using Excursionistas.API.Middleware;
using Excursionistas.Infrastructure.Data;
using Excursionistas.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;

// Cargar variables de entorno desde archivo .env
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}
else
{
    // Intentar cargar desde el directorio actual (para Docker)
    var currentDirEnv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(currentDirEnv))
    {
        Env.Load(currentDirEnv);
    }
}

var builder = WebApplication.CreateBuilder(args);

// Configurar puerto desde variables de entorno
var apiPort = Environment.GetEnvironmentVariable("API_PORT") ?? "5000";
var apiHttpsPort = Environment.GetEnvironmentVariable("API_HTTPS_PORT") ?? "5001";

// Si ASPNETCORE_URLS está definida, usarla (para Docker)
var aspnetcoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrEmpty(aspnetcoreUrls))
{
    builder.WebHost.UseUrls(aspnetcoreUrls);
}
else
{
    // Solo en desarrollo local usar HTTPS
    if (builder.Environment.IsDevelopment())
    {
        builder.WebHost.UseUrls($"http://localhost:{apiPort}", $"https://localhost:{apiHttpsPort}");
    }
    else
    {
        builder.WebHost.UseUrls($"http://localhost:{apiPort}");
    }
}

// Configuración de servicios
builder.Services.AddControllersConfiguration();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddFluentValidationConfiguration();
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Aplicar migraciones y seeding de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ExcursionistasDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // Aplicar migraciones pendientes (solo si no es InMemory)
        var isInMemory = dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";

        if (!isInMemory)
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("✅ Migraciones aplicadas exitosamente");
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("✅ Base de datos en memoria creada exitosamente");
        }

        // Seeding de datos si está habilitado
        var enableSeeding = Environment.GetEnvironmentVariable("ENABLE_DATA_SEEDING") == "true";
        if (enableSeeding && !isInMemory) // No seed en tests
        {
            await DatabaseSeeder.SeedAsync(dbContext);
            logger.LogInformation("✅ Datos iniciales cargados exitosamente");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error al inicializar la base de datos");
        throw;
    }
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Excursionistas API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

// Middleware personalizado para manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();

app.MapControllers();

var databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? "Sqlite";
app.Logger.LogInformation("🚀 Excursionistas API iniciada exitosamente");
app.Logger.LogInformation("🗄️  Proveedor de base de datos: {DatabaseProvider}", databaseProvider);
app.Logger.LogInformation("📚 Documentación Swagger disponible en: http://localhost:{Port}", apiPort);

app.Run();

// Hacer que Program sea accesible para los tests de integración
public partial class Program { }
