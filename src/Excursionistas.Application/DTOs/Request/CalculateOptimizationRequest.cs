namespace Excursionistas.Application.DTOs.Request;

/// <summary>
/// DTO para solicitar el cálculo de optimización.
/// </summary>
public class CalculateOptimizationRequest
{
    /// <summary>
    /// IDs de los elementos disponibles para la optimización.
    /// Si está vacío, se utilizarán todos los elementos activos.
    /// </summary>
    public List<int> ElementIds { get; set; } = new();

    /// <summary>
    /// Calorías mínimas requeridas.
    /// </summary>
    public decimal MinimumCalories { get; set; }

    /// <summary>
    /// Peso máximo que se puede transportar.
    /// </summary>
    public decimal MaximumWeight { get; set; }
}