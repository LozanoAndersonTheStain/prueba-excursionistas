using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Application.Interfaces;
using Excursionistas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Excursionistas.API.Controllers;

/// <summary>
/// Controlador para realizar cálculos de optimización de elementos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OptimizationController : ControllerBase
{
    private readonly IOptimizationService _optimizationService;
    private readonly ILogger<OptimizationController> _logger;

    public OptimizationController(
        IOptimizationService optimizationService,
        ILogger<OptimizationController> logger)
    {
        _optimizationService = optimizationService ?? throw new ArgumentNullException(nameof(optimizationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calcula la combinación óptima de elementos según las restricciones dadas.
    /// El objetivo es alcanzar las calorías mínimas con el menor peso posible,
    /// sin exceder el peso máximo permitido.
    /// </summary>
    /// <param name="request">Parámetros de optimización (IDs de elementos, calorías mínimas, peso máximo).</param>
    /// <returns>Resultado de la optimización con los elementos seleccionados.</returns>
    /// <response code="200">Retorna la solución óptima encontrada.</response>
    /// <response code="400">Si los parámetros son inválidos.</response>
    /// <response code="404">Si no se encuentra solución viable.</response>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(OptimizationResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OptimizationResultResponse>> Calculate(
        [FromBody] CalculateOptimizationRequest request)
    {
        _logger.LogInformation(
            "Calculando optimización para {ElementCount} elementos. Calorías mínimas: {MinCalories}, Peso máximo: {MaxWeight}",
            request.ElementIds?.Count ?? 0,
            request.MinimumCalories,
            request.MaximumWeight);

        try
        {
            var result = await _optimizationService.CalculateOptimizationAsync(request);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Optimización exitosa: {ItemCount} elementos seleccionados, {TotalWeight} peso total, {TotalCalories} calorías totales",
                    result.ItemCount,
                    result.TotalWeight,
                    result.TotalCalories);
            }
            else
            {
                _logger.LogWarning("No se encontró solución de optimización: {Message}", result.Message);
            }

            return Ok(result);
        }
        catch (NoSolutionFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró solución viable: {ErrorMessage}", ex.Message);
            return NotFound(new ErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (InvalidConfigurationException ex)
        {
            _logger.LogWarning(ex, "Configuración inválida: {ErrorMessage}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (InvalidElementException ex)
        {
            _logger.LogWarning(ex, "Elemento inválido: {ErrorMessage}", ex.Message);
            return BadRequest(new ErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Endpoint de salud para verificar que el servicio de optimización está funcionando.
    /// </summary>
    /// <returns>Estado del servicio.</returns>
    /// <response code="200">El servicio está funcionando correctamente.</response>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            Service = "OptimizationService",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}
