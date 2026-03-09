namespace Excursionistas.Application.DTOs.Request;

/// <summary>
/// DTO para crear un nuevo elemento.
/// </summary>
public class CreateElementRequest
{
    /// <summary>
    /// Nombre del elemento.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Peso del elemento (debe ser mayor que 0).
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Calorías proporcionadas por el elemento (deben ser mayores que 0).
    /// </summary>
    public decimal Calories { get; set; }
}