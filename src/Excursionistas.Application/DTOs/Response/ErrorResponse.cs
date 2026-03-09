namespace Excursionistas.Application.DTOs.Response;

/// <summary>
/// DTO de respuesta para errores.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Código de error del dominio.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje descriptivo del error.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Stack trace (solo en desarrollo).
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Errores de validación detallados.
    /// </summary>
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp del error.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
