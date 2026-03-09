namespace Excursionistas.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando el algoritmo de optimización no encuentra una solución viable.
/// </summary>
public class NoSolutionFoundException : DomainException
{
    public NoSolutionFoundException(string message)
        : base(message, "NINGUNA_SOLUCION_ENCONTRADA")
    {
    }

    public NoSolutionFoundException(string message, Exception innerException)
        : base(message, "NINGUNA_SOLUCION_ENCONTRADA", innerException)
    {
    }

    /// <summary>
    /// Crea una excepción para cuando no hay elementos disponibles.
    /// </summary>
    public static NoSolutionFoundException NoItemsAvailable()
    {
        return new NoSolutionFoundException(
            "No se puede calcular una solución sin elementos disponibles");
    }

    /// <summary>
    /// Crea una excepción para cuando ninguna combinación cumple los requisitos.
    /// </summary>
    public static NoSolutionFoundException RequirementsNotMet(
        decimal minimumCalories,
        decimal maximumWeight)
    {
        return new NoSolutionFoundException(
            $"No se encontró ninguna combinación de elementos que cumpla con: " +
            $"Calorías mínimas: {minimumCalories}, Peso máximo: {maximumWeight}");
    }

    /// <summary>
    /// Crea una excepción para cuando todos los elementos exceden el peso máximo.
    /// </summary>
    public static NoSolutionFoundException AllItemsExceedMaxWeight(decimal maximumWeight)
    {
        return new NoSolutionFoundException(
            $"Todos los elementos disponibles exceden el peso máximo permitido: {maximumWeight}");
    }

    /// <summary>
    /// Crea una excepción para cuando no se pueden alcanzar las calorías mínimas.
    /// </summary>
    public static NoSolutionFoundException UnreachableCalories(
        decimal minimumCalories,
        decimal totalAvailableCalories)
    {
        return new NoSolutionFoundException(
            $"No es posible alcanzar {minimumCalories} calorías. " +
            $"Calorías totales disponibles: {totalAvailableCalories}");
    }
}
