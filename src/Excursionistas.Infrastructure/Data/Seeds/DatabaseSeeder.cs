using Excursionistas.Domain.Entities;

namespace Excursionistas.Infrastructure.Data.Seeds;

/// <summary>
/// Clase para inicializar datos en la base de datos.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Siembra datos iniciales en la base de datos si está vacía.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task SeedAsync(ExcursionistasDbContext context)
    {
        // Verificar si ya existen datos
        if (context.Elements.Any() || context.Configurations.Any())
        {
            return; // Ya hay datos, no hacer seeding
        }

        // Seed Elements
        var elements = new List<Element>
        {
            new() { Name = "Botella de agua", Weight = 1.5m, Calories = 500 },
            new() { Name = "Sándwich", Weight = 0.5m, Calories = 800 },
            new() { Name = "Manzana", Weight = 0.3m, Calories = 200 },
            new() { Name = "Banana", Weight = 0.2m, Calories = 150 },
            new() { Name = "Barra energética", Weight = 0.1m, Calories = 300 },
            new() { Name = "Frutos secos", Weight = 0.25m, Calories = 400 },
            new() { Name = "Chocolate", Weight = 0.15m, Calories = 250 },
            new() { Name = "Cuerda (10m)", Weight = 2.0m, Calories = 0 },
            new() { Name = "Mosquetón", Weight = 0.2m, Calories = 0 },
            new() { Name = "Arnés", Weight = 1.0m, Calories = 0 },
            new() { Name = "Casco", Weight = 0.8m, Calories = 0 },
            new() { Name = "Linterna", Weight = 0.3m, Calories = 0 },
            new() { Name = "Botiquín primeros auxilios", Weight = 0.6m, Calories = 0 },
            new() { Name = "Mapa y brújula", Weight = 0.2m, Calories = 0 },
            new() { Name = "Silbato", Weight = 0.05m, Calories = 0 },
            new() { Name = "Manta térmica", Weight = 0.15m, Calories = 0 },
            new() { Name = "Guantes", Weight = 0.2m, Calories = 0 },
            new() { Name = "Gorra", Weight = 0.1m, Calories = 0 },
            new() { Name = "Protector solar", Weight = 0.2m, Calories = 0 },
            new() { Name = "Cantimplora", Weight = 0.4m, Calories = 0 }
        };

        await context.Elements.AddRangeAsync(elements);

        // Seed Configuration
        var configuration = new Excursionistas.Domain.Entities.Configuration
        {
            Name = "Configuración Default",
            MinimumCalories = 1500m,
            MaximumWeight = 10m,
            Description = "Configuración predeterminada para excursiones de día completo"
        };

        await context.Configurations.AddAsync(configuration);

        // Guardar cambios
        await context.SaveChangesAsync();
    }
}
