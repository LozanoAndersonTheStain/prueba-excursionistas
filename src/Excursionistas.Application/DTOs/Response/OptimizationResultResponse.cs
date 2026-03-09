namespace Excursionistas.Application.DTOs.Response;

/// <summary>
/// Response DTO for optimization result.
/// </summary>
public class OptimizationResultResponse
{
    /// <summary>
    /// Indicates whether a viable solution was found.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Descriptive message of the result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Elements selected in the optimal combination.
    /// </summary>
    public List<ElementResponse> SelectedElements { get; set; } = new();

    /// <summary>
    /// Total weight of selected elements.
    /// </summary>
    public decimal TotalWeight { get; set; }

    /// <summary>
    /// Total calories of selected elements.
    /// </summary>
    public decimal TotalCalories { get; set; }

    /// <summary>
    /// Number of elements in the solution.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Average calorie efficiency (calories/weight).
    /// </summary>
    public decimal AverageEfficiency { get; set; }
}
