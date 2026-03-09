using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;

namespace Excursionistas.Application.Interfaces;

/// <summary>
/// Contrato para el servicio de aplicación encargado de gestionar elementos.
/// Define las operaciones CRUD disponibles.
/// </summary>
public interface IElementService
{
    /// <summary>
    /// Obtiene todos los elementos activos del sistema.
    /// </summary>
    Task<IEnumerable<ElementResponse>> GetAllAsync();

    /// <summary>
    /// Obtiene un elemento específico a partir de su identificador.
    /// </summary>
    Task<ElementResponse?> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo elemento en el sistema.
    /// </summary>
    Task<ElementResponse> CreateAsync(CreateElementRequest request);

    /// <summary>
    /// Actualiza la información de un elemento existente.
    /// </summary>
    Task<ElementResponse> UpdateAsync(int id, UpdateElementRequest request);

    /// <summary>
    /// Elimina un elemento utilizando eliminación lógica (soft delete).
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
