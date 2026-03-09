namespace Excursionistas.Domain.Exceptions;

/// <summary>
/// Excepción base para todas las excepciones del dominio.
/// Permite capturar todas las excepciones de negocio de forma centralizada.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Código de error específico del dominio.
    /// </summary>
    public string ErrorCode { get; }

    protected DomainException(string mensaje, string errorCode)
        : base(mensaje)
    {
        ErrorCode = errorCode;
    }

    protected DomainException(string mensaje, string errorCode, Exception innerException)
        : base(mensaje, innerException)
    {
        ErrorCode = errorCode;
    }
}
