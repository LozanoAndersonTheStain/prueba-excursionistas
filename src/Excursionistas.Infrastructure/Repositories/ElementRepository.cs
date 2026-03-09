using Excursionistas.Domain.Entities;
using Excursionistas.Domain.Interfaces;
using Excursionistas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Excursionistas.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la entidad Element.
/// Proporciona acceso a datos usando Entity Framework Core.
/// </summary>
public class ElementRepository : IElementRepository
{
    private readonly ExcursionistasDbContext _context;

    public ElementRepository(ExcursionistasDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene todos los elementos activos de la base de datos.
    /// </summary>
    public async Task<IEnumerable<Element>> GetAllAsync()
    {
        return await _context.Elements
            .Where(e => e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un elemento por su identificador.
    /// Solo devuelve elementos activos (IsActive = true).
    /// </summary>
    public async Task<Element?> GetByIdAsync(int id)
    {
        return await _context.Elements
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
    }

    /// <summary>
    /// Obtiene múltiples elementos por sus identificadores.
    /// </summary>
    public async Task<IEnumerable<Element>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.ToList();

        return await _context.Elements
            .Where(e => idList.Contains(e.Id) && e.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Crea un nuevo elemento en la base de datos.
    /// </summary>
    public async Task<Element> CreateAsync(Element element)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        // Establecer valores por defecto
        element.CreatedAt = DateTime.UtcNow;
        element.UpdatedAt = DateTime.UtcNow;
        element.IsActive = true;

        await _context.Elements.AddAsync(element);
        await _context.SaveChangesAsync();

        return element;
    }

    /// <summary>
    /// Actualiza un elemento existente en la base de datos.
    /// </summary>
    public async Task<Element> UpdateAsync(Element element)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        var existingElement = await GetByIdAsync(element.Id);
        if (existingElement == null)
            throw new InvalidOperationException($"Elemento con Id {element.Id} no encontrado");

        // Actualizar propiedades
        existingElement.Name = element.Name;
        existingElement.Weight = element.Weight;
        existingElement.Calories = element.Calories;
        existingElement.IsActive = element.IsActive;
        existingElement.UpdatedAt = DateTime.UtcNow;

        _context.Elements.Update(existingElement);
        await _context.SaveChangesAsync();

        return existingElement;
    }

    /// <summary>
    /// Elimina (desactiva) un elemento de la base de datos.
    /// Implementa eliminación lógica (soft delete) estableciendo IsActive = false.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var element = await GetByIdAsync(id);
        if (element == null)
            return false;

        // Eliminación lógica
        element.IsActive = false;
        element.UpdatedAt = DateTime.UtcNow;

        _context.Elements.Update(element);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Verifica si existe un elemento con el nombre especificado.
    /// Excluye el elemento con el ID proporcionado (útil para validación en actualizaciones).
    /// </summary>
    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.Elements
            .Where(e => e.Name.ToLower() == name.ToLower() && e.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Obtiene elementos que no excedan el peso máximo especificado.
    /// </summary>
    public async Task<IEnumerable<Element>> GetByMaxWeightAsync(decimal maximumWeight)
    {
        return await _context.Elements
            .Where(e => e.IsActive && e.Weight <= maximumWeight)
            .OrderBy(e => e.Weight)
            .ToListAsync();
    }

    /// <summary>
    /// Guarda todos los cambios pendientes en el contexto.
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
