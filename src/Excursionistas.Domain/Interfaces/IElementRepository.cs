using Excursionistas.Domain.Entities;

namespace Excursionistas.Domain.Interfaces;

/// <summary>
/// Contrato para el repositorio de elementos.
/// Define las operaciones de persistencia para la entidad Element.
/// Sigue el patrón Repository para abstraer el acceso a datos.
/// </summary>
public interface IElementRepository
{
    /// <summary>
    /// Obtiene todos los elementos activos.
    /// </summary>
    Task<IEnumerable<Element>> GetAllAsync();

    /// <summary>
    /// Obtiene un elemento por su identificador.
    /// </summary>
    /// <param name="id">Identificador del elemento.</param>
    /// <returns>El elemento si existe, null en caso contrario.</returns>
    Task<Element?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene múltiples elementos por sus identificadores.
    /// </summary>
    /// <param name="ids">Lista de identificadores.</param>
    /// <returns>Lista de elementos encontrados.</returns>
    Task<IEnumerable<Element>> GetByIdsAsync(IEnumerable<int> ids);

    /// <summary>
    /// Crea un nuevo elemento.
    /// </summary>
    /// <param name="element">Elemento a crear.</param>
    /// <returns>El elemento creado con su ID asignado.</returns>
    Task<Element> CreateAsync(Element element);

    /// <summary>
    /// Actualiza un elemento existente.
    /// </summary>
    /// <param name="element">Elemento con los datos actualizados.</param>
    /// <returns>El elemento actualizado.</returns>
    Task<Element> UpdateAsync(Element element);

    /// <summary>
    /// Elimina un elemento (soft delete).
    /// </summary>
    /// <param name="id">Identificador del elemento a eliminar.</param>
    /// <returns>True si se eliminó correctamente, false si no existe.</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Verifica si existe un elemento con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre del elemento.</param>
    /// <param name="excludeId">ID a excluir de la búsqueda (útil para updates).</param>
    /// <returns>True si existe, false en caso contrario.</returns>
    Task<bool> NameExistsAsync(string name, int? excludeId = null);

    /// <summary>
    /// Obtiene elementos cuyo peso no exceda el límite especificado.
    /// </summary>
    /// <param name="maximumWeight">Peso máximo permitido.</param>
    /// <returns>Lista de elementos que cumplen la condición.</returns>
    Task<IEnumerable<Element>> GetByMaxWeightAsync(decimal maximumWeight);

    /// <summary>
    /// Guarda los cambios pendientes en la base de datos.
    /// </summary>
    /// <returns>Número de registros afectados.</returns>
    Task<int> SaveChangesAsync();
}
