using Excursionistas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Excursionistas.Infrastructure.Configuration;

/// <summary>
/// Configuración de Entity Framework para la entidad Element.
/// Define restricciones, índices y relaciones usando Fluent API.
/// </summary>
public class ElementConfiguration : IEntityTypeConfiguration<Element>
{
    public void Configure(EntityTypeBuilder<Element> builder)
    {
        // Nombre de la tabla
        builder.ToTable("Elements");

        // Clave primaria
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        // Propiedades
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Weight)
            .IsRequired()
            .HasPrecision(18, 2); // Precisión para valores decimales

        builder.Property(e => e.Calories)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Índices para mejorar rendimiento de consultas
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Elements_Name");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Elements_IsActive");

        builder.HasIndex(e => e.Weight)
            .HasDatabaseName("IX_Elements_Weight");

        // Propiedades calculadas ignoradas (no se persisten en BD)
        builder.Ignore(e => e.CalorieEfficiency);
    }
}
