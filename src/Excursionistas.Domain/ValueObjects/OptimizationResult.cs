using Excursionistas.Domain.Entities;

namespace Excursionistas.Domain.ValueObjects;

/// <summary>
/// Representa el resultado de un cálculo de optimización.
/// Este es un Value Object que encapsula el resultado completo de la optimización.
/// </summary>
public class OptimizationResult
{
    /// <summary>
    /// Indica si se encontró una solución viable.
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Lista de elementos seleccionados como óptimos.
    /// </summary>
    public IReadOnlyCollection<Element> SelectedItems { get; private set; }

    /// <summary>
    /// Peso total de los elementos seleccionados.
    /// </summary>
    public decimal TotalWeight { get; private set; }

    /// <summary>
    /// Calorías totales proporcionadas por los elementos seleccionados.
    /// </summary>
    public decimal TotalCalories { get; private set; }

    /// <summary>
    /// Mensaje descriptivo del resultado.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Número de elementos seleccionados.
    /// </summary>
    public int ItemCount => SelectedItems.Count;

    /// <summary>
    /// Eficiencia calórica promedio de la solución (calorías/peso).
    /// </summary>
    public decimal AverageEfficiency => TotalWeight > 0 ? TotalCalories / TotalWeight : 0;

    private OptimizationResult(
        bool exito,
        IEnumerable<Element> selectedItems,
        decimal totalWeight,
        decimal totalCalories,
        string message)
    {
        Success = exito;
        SelectedItems = selectedItems.ToList().AsReadOnly();
        TotalWeight = totalWeight;
        TotalCalories = totalCalories;
        Message = message;
    }

    /// <summary>
    /// Crea un resultado exitoso de optimización.
    /// </summary>
    public static OptimizationResult CreateSuccessful(
        IEnumerable<Element> elements,
        decimal totalWeight,
        decimal totalCalories,
        string message = "Combinación óptima encontrada")
    {
        return new OptimizationResult(true, elements, totalWeight, totalCalories, message);
    }

    /// <summary>
    /// Crea un resultado fallido de optimización.
    /// </summary>
    public static OptimizationResult CreateFailed(string message)
    {
        return new OptimizationResult(false, Enumerable.Empty<Element>(), 0, 0, message);
    }

    /// <summary>
    /// Crea un resultado a partir de una lista de elementos seleccionados.
    /// Calcula automáticamente el peso y calorías totales.
    /// </summary>
    public static OptimizationResult FromItems(IEnumerable<Element> elements, string? personalizedMessage = null)
    {
        var elementosLista = elements.ToList();
        var totalWeight = elementosLista.Sum(e => e.Weight);
        var totalCalories = elementosLista.Sum(e => e.Calories);
        var mensaje = personalizedMessage ??
                     $"Se seleccionaron {elementosLista.Count} elementos con {totalCalories} calorías y {totalWeight} de peso";

        return new OptimizationResult(true, elementosLista, totalWeight, totalCalories, mensaje);
    }

    public override string ToString()
    {
        if (!Success)
            return $"❌ {Message}";

        return $"✅ {Message} | Elementos: {ItemCount} | Peso: {TotalWeight} | Calorías: {TotalCalories}";
    }
}
