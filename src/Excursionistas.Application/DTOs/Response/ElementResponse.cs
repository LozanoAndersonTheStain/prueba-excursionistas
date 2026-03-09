namespace Excursionistas.Application.DTOs.Response;

/// <summary>
/// Response DTO for an element.
/// </summary>
public class ElementResponse
{
    /// <summary>
    /// Unique identifier of the element.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Element name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Element weight.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Element calories.
    /// </summary>
    public decimal Calories { get; set; }

    /// <summary>
    /// Calorie efficiency (calories/weight).
    /// </summary>
    public decimal CalorieEfficiency { get; set; }

    /// <summary>
    /// Creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the element is active.
    /// </summary>
    public bool IsActive { get; set; }
}
