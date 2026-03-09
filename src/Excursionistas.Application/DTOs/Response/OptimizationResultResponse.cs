namespace Excursionistas.Application.DTOs.Response;

/// <summary>
/// DTO de respuesta que representa el resultado de un cálculo de optimización.
/// </summary>
public class OptimizationResultResponse
{
    /// <summary>
    /// Indica si se encontró una solución viable para la optimización.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje descriptivo del resultado de la optimización.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Lista de elementos seleccionados en la combinación óptima.
    /// </summary>
    public List<ElementResponse> SelectedElements { get; set; } = new();

    /// <summary>
    /// Peso total de los elementos seleccionados.
    /// </summary>
    public decimal TotalWeight { get; set; }

    /// <summary>
    /// Calorías totales aportadas por los elementos seleccionados.
    /// </summary>
    public decimal TotalCalories { get; set; }

    /// <summary>
    /// Cantidad de elementos incluidos en la solución óptima.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Eficiencia calórica promedio de la combinación seleccionada.
    /// (calorías/peso).
    /// </summary>
    public decimal AverageEfficiency { get; set; }
}
