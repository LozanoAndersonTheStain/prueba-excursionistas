using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Excursionistas.Infrastructure.Data;

/// <summary>
/// Factory para crear instancias de ExcursionistasDbContext en tiempo de diseño.
/// Esta clase es utilizada por las herramientas de Entity Framework Core (migrations, etc.)
/// </summary>
public class ExcursionistasDbContextFactory : IDesignTimeDbContextFactory<ExcursionistasDbContext>
{
    /// <summary>
    /// Crea una instancia del DbContext configurada según el proveedor especificado en .env
    /// Utilizada por las herramientas de EF Core durante el desarrollo.
    /// </summary>
    public ExcursionistasDbContext CreateDbContext(string[] args)
    {
        // Intentar cargar el archivo .env desde varios posibles directorios
        LoadEnvironmentVariables();

        var optionsBuilder = new DbContextOptionsBuilder<ExcursionistasDbContext>();

        // Leer el proveedor de base de datos desde variables de entorno
        var databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? "Sqlite";

        if (databaseProvider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
        {
            // Configurar PostgreSQL para migraciones
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
            var database = Environment.GetEnvironmentVariable("POSTGRES_DATABASE") ?? "excursionistas_db";
            var username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "";

            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
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
            // Configurar SQLite para migraciones (por defecto)
            var connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING")
                ?? "Data Source=excursionistas.db";

            optionsBuilder.UseSqlite(connectionString);
        }

        return new ExcursionistasDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Intenta cargar el archivo .env desde varios directorios posibles.
    /// </summary>
    private static void LoadEnvironmentVariables()
    {
        try
        {
            // Intentar desde el directorio actual
            var currentDir = Directory.GetCurrentDirectory();
            var envPath = Path.Combine(currentDir, ".env");

            if (File.Exists(envPath))
            {
                Env.Load(envPath);
                return;
            }

            // Intentar desde el directorio raíz del proyecto (dos niveles arriba de src)
            var projectRoot = Path.Combine(currentDir, "..", "..", ".env");
            if (File.Exists(projectRoot))
            {
                Env.Load(projectRoot);
                return;
            }

            // Intentar desde el directorio de la solución (tres niveles arriba)
            var solutionRoot = Path.Combine(currentDir, "..", "..", "..", ".env");
            if (File.Exists(solutionRoot))
            {
                Env.Load(solutionRoot);
            }
        }
        catch
        {
            // Si falla la carga del .env, las variables de entorno del sistema se usarán
        }
    }
}
