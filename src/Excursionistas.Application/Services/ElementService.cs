using AutoMapper;
using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Application.Interfaces;
using Excursionistas.Domain.Entities;
using Excursionistas.Domain.Exceptions;
using Excursionistas.Domain.Interfaces;

namespace Excursionistas.Application.Services;

/// <summary>
/// Servicio de aplicación encargado de la gestión de elementos.
/// Implementa la lógica de negocio y coordina las operaciones
/// entre el repositorio y el dominio.
/// </summary>
public class ElementService : IElementService
{
    private readonly IElementRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor del servicio que inyecta dependencias necesarias.
    /// </summary>
    public ElementService(IElementRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Obtiene todos los elementos activos del repositorio.
    /// </summary>
    public async Task<IEnumerable<ElementResponse>> GetAllAsync()
    {
        var elements = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ElementResponse>>(elements);
    }

    /// <summary>
    /// Obtiene un elemento específico a partir de su identificador.
    /// </summary>
    public async Task<ElementResponse?> GetByIdAsync(int id)
    {
        var element = await _repository.GetByIdAsync(id);
        return element == null ? null : _mapper.Map<ElementResponse>(element);
    }

    /// <summary>
    /// Crea un nuevo elemento después de validar reglas de negocio.
    /// </summary>
    public async Task<ElementResponse> CreateAsync(CreateElementRequest request)
    {
        // Validate that an element with the same name doesn't exist
        if (await _repository.NameExistsAsync(request.Name))
        {
            throw new InvalidElementException($"An element with the name '{request.Name}' already exists");
        }

        var element = _mapper.Map<Element>(request);

        // Validate element using domain logic
        if (!element.IsValid())
        {
            throw new InvalidElementException("The element does not meet validation rules");
        }

        var createdElement = await _repository.CreateAsync(element);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ElementResponse>(createdElement);
    }

    /// <summary>
    /// Actualiza un elemento existente después de validar su existencia y reglas de negocio.
    /// </summary>
    public async Task<ElementResponse> UpdateAsync(int id, UpdateElementRequest request)
    {
        var existingElement = await _repository.GetByIdAsync(id);
        if (existingElement == null)
        {
            throw InvalidElementException.NotFound(id);
        }

        // Validate that another element with the same name doesn't exist
        if (await _repository.NameExistsAsync(request.Name, id))
        {
            throw new InvalidElementException($"Another element with the name '{request.Name}' already exists");
        }

        // Map changes to existing element
        _mapper.Map(request, existingElement);

        // Validate updated element
        if (!existingElement.IsValid())
        {
            throw new InvalidElementException("The updated element does not meet validation rules");
        }

        var updatedElement = await _repository.UpdateAsync(existingElement);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ElementResponse>(updatedElement);
    }

    /// <summary>
    /// Elimina un elemento utilizando eliminación lógica (soft delete).
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await _repository.SaveChangesAsync();
        }
        return result;
    }
}
