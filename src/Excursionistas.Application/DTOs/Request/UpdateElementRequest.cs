namespace Excursionistas.Application.DTOs.Request;

/// <summary>
/// DTO para actualizar un elemento existente.
/// Extiende CreateElementRequest con propiedades adicionales para las actualizaciones.
/// </summary>
public class UpdateElementRequest : CreateElementRequest
{
    /// <summary>
    /// Indica si el elemento debe estar activo.
    /// </summary>
    public bool IsActive { get; set; } = true;
}