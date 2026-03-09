namespace Excursionistas.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando un elemento no cumple con las reglas de validación del dominio.
/// </summary>
public class InvalidElementException : DomainException
{
    public InvalidElementException(string message)
        : base(message, "ELEMENTO_INVALIDO")
    {
    }

    public InvalidElementException(string message, Exception innerException)
        : base(message, "ELEMENTO_INVALIDO", innerException)
    {
    }

    /// <summary>
    /// Crea una excepción para cuando el nombre está vacío.
    /// </summary>
    public static InvalidElementException EmptyName()
    {
        return new InvalidElementException("El nombre del elemento no puede estar vacío");
    }

    /// <summary>
    /// Crea una excepción para cuando el peso es inválido.
    /// </summary>
    public static InvalidElementException InvalidWeight(decimal weight)
    {
        return new InvalidElementException($"El peso del elemento debe ser mayor a 0. Valor recibido: {weight}");
    }

    /// <summary>
    /// Crea una excepción para cuando las calorías son inválidas.
    /// </summary>
    public static InvalidElementException InvalidCalories(decimal calories)
    {
        return new InvalidElementException($"Las calorías del elemento deben ser mayores a 0. Valor recibido: {calories}");
    }

    /// <summary>
    /// Crea una excepción para cuando el elemento no existe.
    /// </summary>
    public static InvalidElementException NotFound(int id)
    {
        return new InvalidElementException($"No se encontró el elemento con ID: {id}");
    }
}
