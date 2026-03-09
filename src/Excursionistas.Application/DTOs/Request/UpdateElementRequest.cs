namespace Excursionistas.Application.DTOs.Request;

/// <summary>
/// DTO for updating an existing element.
/// Extends CreateElementRequest with additional properties for updates.
/// </summary>
public class UpdateElementRequest : CreateElementRequest
{
    /// <summary>
    /// Indicates whether the element should be active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
