using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainConfiguration = Excursionistas.Domain.Entities.Configuration;

namespace Excursionistas.Infrastructure.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad Configuration.
/// Define restricciones, índices y relaciones usando Fluent API.
/// </summary>
public class ConfigurationEntityConfiguration : IEntityTypeConfiguration<DomainConfiguration>
{
    public void Configure(EntityTypeBuilder<DomainConfiguration> builder)
    {
        // Nombre de la tabla
        builder.ToTable("Configurations");

        // Clave primaria
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        // Propiedades
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.MinimumCalories)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.MaximumWeight)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices para mejorar rendimiento de consultas
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_Configurations_Name");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Configurations_IsActive");
    }
}
