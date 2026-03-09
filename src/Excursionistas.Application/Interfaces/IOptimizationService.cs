using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;

namespace Excursionistas.Application.Interfaces;

/// <summary>
/// Contrato para el servicio de aplicación encargado del cálculo de optimización.
/// Coordina el uso de los servicios de dominio responsables del algoritmo.
/// </summary>
public interface IOptimizationService
{
    /// <summary>
    /// Calcula la combinación óptima de elementos según los parámetros enviados.
    /// </summary>
    Task<OptimizationResultResponse> CalculateOptimizationAsync(CalculateOptimizationRequest request);
}
