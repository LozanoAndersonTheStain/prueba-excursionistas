using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Application.Interfaces;
using Excursionistas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Excursionistas.API.Controllers;

/// <summary>
/// Controlador para gestionar operaciones CRUD sobre elementos de equipo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ElementsController : ControllerBase
{
    private readonly IElementService _elementService;
    private readonly ILogger<ElementsController> _logger;

    public ElementsController(
        IElementService elementService,
        ILogger<ElementsController> logger)
    {
        _elementService = elementService ?? throw new ArgumentNullException(nameof(elementService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los elementos activos.
    /// </summary>
    /// <returns>Lista de elementos.</returns>
    /// <response code="200">Retorna la lista de elementos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ElementResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ElementResponse>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los elementos");
        var elements = await _elementService.GetAllAsync();
        return Ok(elements);
    }

    /// <summary>
    /// Obtiene un elemento específico por su ID.
    /// </summary>
    /// <param name="id">Identificador del elemento.</param>
    /// <returns>El elemento solicitado.</returns>
    /// <response code="200">Retorna el elemento solicitado.</response>
    /// <response code="404">Si el elemento no existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ElementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ElementResponse>> GetById(int id)
    {
        _logger.LogInformation("Obteniendo elemento con ID: {ElementId}", id);
        var element = await _elementService.GetByIdAsync(id);

        if (element == null)
        {
            _logger.LogWarning("Elemento con ID {ElementId} no encontrado", id);
            return NotFound(new ErrorResponse
            {
                ErrorCode = "ELEMENT_NOT_FOUND",
                Message = $"Elemento con ID {id} no encontrado",
                Timestamp = DateTime.UtcNow
            });
        }

        return Ok(element);
    }

    /// <summary>
    /// Crea un nuevo elemento.
    /// </summary>
    /// <param name="request">Datos del elemento a crear.</param>
    /// <returns>El elemento creado.</returns>
    /// <response code="201">Retorna el elemento recién creado.</response>
    /// <response code="400">Si los datos son inválidos.</response>
    /// <response code="409">Si ya existe un elemento con ese nombre.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ElementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ElementResponse>> Create([FromBody] CreateElementRequest request)
    {
        _logger.LogInformation("Creando nuevo elemento: {ElementName}", request.Name);

        try
        {
            var element = await _elementService.CreateAsync(request);
            _logger.LogInformation("Elemento creado exitosamente con ID: {ElementId}", element.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = element.Id },
                element);
        }
        catch (InvalidElementException ex)
        {
            _logger.LogWarning(ex, "Error al crear elemento: {ErrorMessage}", ex.Message);

            if (ex.Message.Contains("ya existe", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ErrorResponse
                {
                    ErrorCode = ex.ErrorCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }

            return BadRequest(new ErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Actualiza un elemento existente.
    /// </summary>
    /// <param name="id">Identificador del elemento a actualizar.</param>
    /// <param name="request">Datos actualizados del elemento.</param>
    /// <returns>El elemento actualizado.</returns>
    /// <response code="200">Retorna el elemento actualizado.</response>
    /// <response code="400">Si los datos son inválidos.</response>
    /// <response code="404">Si el elemento no existe.</response>
    /// <response code="409">Si ya existe otro elemento con ese nombre.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ElementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ElementResponse>> Update(int id, [FromBody] UpdateElementRequest request)
    {
        _logger.LogInformation("Actualizando elemento con ID: {ElementId}", id);

        try
        {
            var element = await _elementService.UpdateAsync(id, request);
            _logger.LogInformation("Elemento {ElementId} actualizado exitosamente", id);
            return Ok(element);
        }
        catch (InvalidElementException ex)
        {
            _logger.LogWarning(ex, "Error al actualizar elemento {ElementId}: {ErrorMessage}", id, ex.Message);

            if (ex.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse
                {
                    ErrorCode = ex.ErrorCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (ex.Message.Contains("ya existe", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ErrorResponse
                {
                    ErrorCode = ex.ErrorCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }

            return BadRequest(new ErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Elimina un elemento (soft delete).
    /// </summary>
    /// <param name="id">Identificador del elemento a eliminar.</param>
    /// <returns>Confirmación de eliminación.</returns>
    /// <response code="204">Elemento eliminado exitosamente.</response>
    /// <response code="404">Si el elemento no existe.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando elemento con ID: {ElementId}", id);

        var result = await _elementService.DeleteAsync(id);

        if (!result)
        {
            _logger.LogWarning("Elemento con ID {ElementId} no encontrado para eliminar", id);
            return NotFound(new ErrorResponse
            {
                ErrorCode = "ELEMENT_NOT_FOUND",
                Message = $"Elemento con ID {id} no encontrado",
                Timestamp = DateTime.UtcNow
            });
        }

        _logger.LogInformation("Elemento {ElementId} eliminado exitosamente", id);
        return NoContent();
    }
}
