using Excursionistas.Domain.Entities;
using Excursionistas.Domain.ValueObjects;

namespace Excursionistas.Domain.Interfaces;

/// <summary>
/// Contrato para el servicio de optimización de elementos.
/// Define el algoritmo que determina la combinación óptima de elementos.
/// Sigue el principio de Inversión de Dependencias (DIP) de SOLID.
/// </summary>
public interface IOptimizerService
{
    /// <summary>
    /// Calcula la combinación óptima de elementos basándose en las restricciones dadas.
    /// El objetivo es cumplir con el mínimo de calorías mientras se minimiza el peso total.
    /// </summary>
    /// <param name="elements">Lista de elementos disponibles para seleccionar.</param>
    /// <param name="minimumCalories">Cantidad mínima de calorías requeridas.</param>
    /// <param name="maximumWeight">Peso máximo que se puede llevar.</param>
    /// <returns>Resultado de la optimización con los elementos seleccionados.</returns>
    Task<OptimizationResult> CalculateOptimizationAsync(
        IEnumerable<Element> elements,
        decimal minimumCalories,
        decimal maximumWeight);

    /// <summary>
    /// Calcula la combinación óptima usando una configuración específica.
    /// </summary>
    /// <param name="elements">Lista de elementos disponibles.</param>
    /// <param name="configuration">Configuración con las restricciones.</param>
    /// <returns>Resultado de la optimización.</returns>
    Task<OptimizationResult> CalculateOptimizationAsync(
        IEnumerable<Element> elements,
        Configuration configuration);

    /// <summary>
    /// Valida que los elementos y la configuración sean válidos para el cálculo.
    /// </summary>
    /// <param name="elements">Elementos a validar.</param>
    /// <param name="minimumCalories">Calorías mínimas requeridas.</param>
    /// <param name="maximumWeight">Peso máximo permitido.</param>
    /// <returns>True si la validación es exitosa.</returns>
    /// <exception cref="Excursionistas.Domain.Exceptions.InvalidConfigurationException">
    /// Si la configuración no es válida.
    /// </exception>
    /// <exception cref="Excursionistas.Domain.Exceptions.InvalidElementException">
    /// Si algún elemento no es válido.
    /// </exception>
    Task<bool> ValidateInputAsync(
        IEnumerable<Element> elements,
        decimal minimumCalories,
        decimal maximumWeight);
}
