namespace Excursionistas.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una configuración no cumple con las reglas de validación del dominio.
/// </summary>
public class InvalidConfigurationException : DomainException
{
    public InvalidConfigurationException(string message)
        : base(message, "CONFIGURACION_INVALIDA")
    {
    }

    public InvalidConfigurationException(string message, Exception innerException)
        : base(message, "CONFIGURACION_INVALIDA", innerException)
    {
    }

    /// <summary>
    /// Crea una excepción para cuando las calorías mínimas son inválidas.
    /// </summary>
    public static InvalidConfigurationException InvalidMinimumCalories(decimal calories)
    {
        return new InvalidConfigurationException(
            $"Las calorías mínimas deben ser mayores a 0. Valor recibido: {calories}");
    }

    /// <summary>
    /// Crea una excepción para cuando el peso máximo es inválido.
    /// </summary>
    public static InvalidConfigurationException InvalidMaximumWeight(decimal weight)
    {
        return new InvalidConfigurationException(
            $"El peso máximo debe ser mayor a 0. Valor recibido: {weight}");
    }

    /// <summary>
    /// Crea una excepción para cuando la configuración es inconsistente.
    /// </summary>
    public static InvalidConfigurationException InconsistentConfiguration(string reason)
    {
        return new InvalidConfigurationException(
            $"La configuración es inconsistente: {reason}");
    }
}
