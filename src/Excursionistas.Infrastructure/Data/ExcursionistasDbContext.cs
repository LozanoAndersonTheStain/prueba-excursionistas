using Excursionistas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using DomainConfiguration = Excursionistas.Domain.Entities.Configuration;

namespace Excursionistas.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para la aplicación Excursionistas.
/// Gestiona las entidades Element y Configuration utilizando Entity Framework Core.
/// </summary>
public class ExcursionistasDbContext : DbContext
{
    /// <summary>
    /// Conjunto de entidades Element (elementos de equipo).
    /// </summary>
    public DbSet<Element> Elements { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Configuration (configuraciones de optimización).
    /// </summary>
    public DbSet<DomainConfiguration> Configurations { get; set; } = null!;

    /// <summary>
    /// Constructor que recibe las opciones de configuración del contexto.
    /// </summary>
    public ExcursionistasDbContext(DbContextOptions<ExcursionistasDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configura el modelo de datos usando Fluent API.
    /// Aplica todas las configuraciones definidas en el ensamblado actual.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas las configuraciones de entidades del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Sobrescribe SaveChanges para actualizar automáticamente las fechas de auditoría.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Sobrescribe SaveChangesAsync para actualizar automáticamente las fechas de auditoría.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza los campos de auditoría (CreatedAt, UpdatedAt) antes de guardar cambios.
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Actualizar UpdatedAt para elementos modificados
            if (entry.State == EntityState.Modified && entry.Entity is Element element)
            {
                element.UpdatedAt = DateTime.UtcNow;
            }

            // Establecer CreatedAt para nuevos elementos
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is Element newElement)
                {
                    newElement.CreatedAt = DateTime.UtcNow;
                    newElement.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is DomainConfiguration newConfig)
                {
                    newConfig.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
