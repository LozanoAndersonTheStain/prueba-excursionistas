namespace Excursionistas.Domain.Entities;

/// <summary>
/// Representa un elemento que puede ser llevado durante la escalada.
/// Cada elemento tiene un nombre, peso y cantidad de calorías que proporciona.
/// </summary>
public class Element
{
    /// <summary>
    /// Identificador único del elemento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del elemento (ej: "E1", "Barrita energética", etc.).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Peso del elemento en la unidad definida (ej: kilogramos, libras).
    /// Debe ser mayor a 0.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Cantidad de calorías que proporciona el elemento.
    /// Debe ser mayor a 0.
    /// </summary>
    public decimal Calories { get; set; }

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización del registro.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica si el elemento está activo (soft delete).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Calcula la relación calorías/peso del elemento (eficiencia calórica).
    /// Valores más altos indican elementos más eficientes.
    /// </summary>
    public decimal CalorieEfficiency => Weight > 0 ? Calories / Weight : 0;

    /// <summary>
    /// Valida que el elemento tenga datos válidos.
    /// </summary>
    /// <returns>True si el elemento es válido, false en caso contrario.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name)
            && Weight > 0
            && Calories > 0;
    }

    public override string ToString()
    {
        return $"{Name} - Peso: {Weight}, Calorías: {Calories}";
    }
}
