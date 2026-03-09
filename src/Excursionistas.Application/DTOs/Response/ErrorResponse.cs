namespace Excursionistas.Application.DTOs.Response;

/// <summary>
/// DTO utilizado para representar respuestas de error en la API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Código de error proveniente del dominio o la aplicación.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje descriptivo del error ocurrido.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Stack trace del error (solo se incluye en entornos de desarrollo).
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Lista detallada de errores de validación por campo.
    /// </summary>
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }

    /// <summary>
    /// Fecha y hora en que ocurrió el error.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
