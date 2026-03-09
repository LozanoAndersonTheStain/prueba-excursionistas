using AutoMapper;
using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Application.Interfaces;
using Excursionistas.Domain.Interfaces;

namespace Excursionistas.Application.Services;

/// <summary>
/// Servicio de aplicación encargado de ejecutar el cálculo de optimización.
/// Coordina la obtención de datos desde el repositorio y delega el algoritmo
/// de optimización al servicio de dominio correspondiente.
/// </summary>
public class OptimizationService : IOptimizationService
{
    private readonly IElementRepository _repository;
    private readonly IOptimizerService _optimizerService;
    private readonly IMapper _mapper;

    public OptimizationService(
        IElementRepository repository,
        IOptimizerService optimizerService,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _optimizerService = optimizerService ?? throw new ArgumentNullException(nameof(optimizerService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Ejecuta el cálculo de optimización con los parámetros recibidos.
    /// </summary>
    public async Task<OptimizationResultResponse> CalculateOptimizationAsync(CalculateOptimizationRequest request)
    {
        // Obtener elementos: si se especificaron ID, esos seran los que se utilicen; de lo contrario, se obtienen todos.
        var elements = request.ElementIds != null && request.ElementIds.Any()
            ? await _repository.GetByIdsAsync(request.ElementIds)
            : await _repository.GetAllAsync();

        // Validar input antes de ejecutar el algoritmo
        await _optimizerService.ValidateInputAsync(
            elements,
            request.MinimumCalories,
            request.MaximumWeight);

        // Ejecutar el algoritmo de optimización con los parámetros proporcionados
        var result = await _optimizerService.CalculateOptimizationAsync(
            elements,
            request.MinimumCalories,
            request.MaximumWeight);

        // Mapear el resultado del dominio a un DTO de respuesta para la capa de aplicación
        return _mapper.Map<OptimizationResultResponse>(result);
    }
}
