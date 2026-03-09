namespace Excursionistas.Application.DTOs.Response;

/// <summary>
/// DTO de respuesta que representa un elemento dentro del sistema.
/// Se utiliza para enviar información del elemento hacia el cliente.
/// </summary>
public class ElementResponse
{
    /// <summary>
    /// Identificador único del elemento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del elemento.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Peso del elemento.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Cantidad de calorías que aporta el elemento.
    /// </summary>
    public decimal Calories { get; set; }

    /// <summary>
    /// Eficiencia calórica del elemento (calorías/peso).
    /// </summary>
    public decimal CalorieEfficiency { get; set; }

    /// <summary>
    /// Fecha de creación del elemento.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indica si el elemento está activo dentro del sistema.
    /// </summary>
    public bool IsActive { get; set; }
}
