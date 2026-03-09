using Excursionistas.Application.Interfaces;
using Excursionistas.Application.Mappings;
using Excursionistas.Application.Services;
using Excursionistas.Application.Validators;
using Excursionistas.Domain.Interfaces;
using Excursionistas.Domain.Services;
using Excursionistas.Infrastructure.Data;
using Excursionistas.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace Excursionistas.API.Extensions;

/// <summary>
/// Extensiones para configurar servicios en el contenedor de DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura el DbContext con el proveedor de base de datos especificado (SQLite o PostgreSQL).
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER")
            ?? configuration.GetValue<string>("DatabaseProvider")
            ?? "Sqlite";

        var enableSensitiveDataLogging = Environment.GetEnvironmentVariable("ENABLE_SENSITIVE_DATA_LOGGING") == "true"
            || configuration.GetValue<bool>("EnableSensitiveDataLogging");

        services.AddDbContext<ExcursionistasDbContext>(options =>
        {
            if (databaseProvider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
            {
                // PostgreSQL Configuration
                var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
                var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
                var database = Environment.GetEnvironmentVariable("POSTGRES_DATABASE") ?? "excursionistas_db";
                var username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
                var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "";

                var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
                    ?? $"Host={host};Port={port};Database={database};Username={username};Password={password}";

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("Excursionistas.Infrastructure");
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                });
            }
            else
            {
                // SQLite Configuration (default)
                var connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING")
                    ?? configuration.GetConnectionString("DefaultConnection")
                    ?? "Data Source=excursionistas.db";

                options.UseSqlite(connectionString);
            }

            // Habilitar logging sensible solo en desarrollo
            if (enableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    /// <summary>
    /// Registra los repositorios en el contenedor de DI.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IElementRepository, ElementRepository>();

        return services;
    }

    /// <summary>
    /// Registra los servicios de aplicación en el contenedor de DI.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IElementService, ElementService>();
        services.AddScoped<IOptimizationService, OptimizationService>();

        return services;
    }

    /// <summary>
    /// Registra los servicios de dominio en el contenedor de DI.
    /// </summary>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IOptimizerService, OptimizerService>();

        return services;
    }

    /// <summary>
    /// Configura AutoMapper con los profiles de la aplicación.
    /// </summary>
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }

    /// <summary>
    /// Configura FluentValidation para validación de DTOs.
    /// </summary>
    public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        // Registrar todos los validadores del ensamblado Application
        services.AddValidatorsFromAssemblyContaining<CreateElementRequestValidator>();

        return services;
    }

    /// <summary>
    /// Configura CORS para permitir solicitudes desde orígenes específicos.
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Leer desde variables de entorno o configuración
        var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
        string[] allowedOrigins;

        if (!string.IsNullOrEmpty(corsOriginsEnv))
        {
            // Separar por comas desde variable de entorno
            allowedOrigins = corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
        else
        {
            // Leer desde appsettings
            allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:3000", "http://localhost:4200", "http://localhost:5173" };
        }

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Configura controladores con opciones personalizadas.
    /// </summary>
    public static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy =
                    System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        return services;
    }

    /// <summary>
    /// Configura Swagger/OpenAPI para documentación de la API.
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Excursionistas API",
                Version = "v1",
                Description = "API para optimización de selección de equipo de escalada basado en restricciones de peso y calorías",
                Contact = new()
                {
                    Name = "Excursionistas Team",
                    Url = new Uri("https://github.com/LozanoAndersonTheStain/prueba-excursionistas")
                }
            });

            // Incluir comentarios XML si están disponibles
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
