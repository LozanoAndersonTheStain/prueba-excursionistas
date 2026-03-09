namespace Excursionistas.Domain.Entities;

/// <summary>
/// Representa la configuración de restricciones para una expedición de escalada.
/// Define el mínimo de calorías requeridas y el peso máximo permitido.
/// </summary>
public class Configuration
{
    /// <summary>
    /// Identificador único de la configuración.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre descriptivo de la configuración.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad mínima de calorías que debe cumplir la selección de elementos.
    /// </summary>
    public decimal MinimumCalories { get; set; }

    /// <summary>
    /// Peso máximo que puede llevar el excursionista.
    /// </summary>
    public decimal MaximumWeight { get; set; }

    /// <summary>
    /// Descripción adicional de la configuración.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si la configuración está activa.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Valida que la configuración tenga valores válidos.
    /// </summary>
    /// <returns>True si la configuración es válida, false en caso contrario.</returns>
    public bool IsValid()
    {
        return MinimumCalories > 0
            && MaximumWeight > 0
            && !string.IsNullOrWhiteSpace(Name);
    }

    public override string ToString()
    {
        return $"{Name} - Min Calorías: {MinimumCalories}, Max Peso: {MaximumWeight}";
    }
}
