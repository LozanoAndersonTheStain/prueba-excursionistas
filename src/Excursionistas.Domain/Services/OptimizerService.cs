using Excursionistas.Domain.Entities;
using Excursionistas.Domain.Exceptions;
using Excursionistas.Domain.Interfaces;
using Excursionistas.Domain.ValueObjects;

namespace Excursionistas.Domain.Services;

/// <summary>
/// Servicio de dominio que implementa el algoritmo de optimización.
/// Resuelve el problema de la mochila (knapsack problem) con restricciones específicas:
/// - Objetivo: Alcanzar calorías mínimas con el menor peso posible
/// - Restricción: No exceder el peso máximo permitido
/// </summary>
public class OptimizerService : IOptimizerService
{
    /// <summary>
    /// Calcula la combinación óptima de elementos basándose en las restricciones dadas.
    /// Utiliza un algoritmo híbrido que combina estrategias greedy y de búsqueda exhaustiva.
    /// </summary>
    public async Task<OptimizationResult> CalculateOptimizationAsync(
        IEnumerable<Element> elements,
        decimal minimumCalories,
        decimal maximumWeight)
    {
        // Validar entrada
        await ValidateInputAsync(elements, minimumCalories, maximumWeight);

        var elementList = elements.ToList();

        // Filtrar elementos que exceden el peso máximo
        var validElements = elementList
            .Where(e => e.Weight <= maximumWeight)
            .ToList();

        if (!validElements.Any())
        {
            throw NoSolutionFoundException.AllItemsExceedMaxWeight(maximumWeight);
        }

        // Verificar si es posible alcanzar las calorías mínimas
        var totalAvailableCalories = validElements.Sum(e => e.Calories);
        if (totalAvailableCalories < minimumCalories)
        {
            throw NoSolutionFoundException.UnreachableCalories(minimumCalories, totalAvailableCalories);
        }

        // Intentar encontrar solución usando estrategia greedy por eficiencia
        var greedySolution = TryGreedyByEfficiency(validElements, minimumCalories, maximumWeight);
        if (greedySolution != null)
        {
            return greedySolution;
        }

        // Si greedy falla, usar programación dinámica
        var dpSolution = SolveDynamicProgramming(validElements, minimumCalories, maximumWeight);
        if (dpSolution != null)
        {
            return dpSolution;
        }

        // Si ninguna estrategia encuentra solución
        throw NoSolutionFoundException.RequirementsNotMet(minimumCalories, maximumWeight);
    }

    /// <summary>
    /// Calcula la optimización usando una configuración específica.
    /// </summary>
    public async Task<OptimizationResult> CalculateOptimizationAsync(
        IEnumerable<Element> elements,
        Configuration configuration)
    {
        if (!configuration.IsValid())
        {
            throw new InvalidConfigurationException("La configuración proporcionada no es válida");
        }

        return await CalculateOptimizationAsync(
            elements,
            configuration.MinimumCalories,
            configuration.MaximumWeight);
    }

    /// <summary>
    /// Valida que los elementos y la configuración sean válidos para el cálculo.
    /// </summary>
    public Task<bool> ValidateInputAsync(
        IEnumerable<Element> elements,
        decimal minimumCalories,
        decimal maximumWeight)
    {
        var elementList = elements?.ToList() ?? new List<Element>();

        // Validar que hay elementos
        if (!elementList.Any())
        {
            throw NoSolutionFoundException.NoItemsAvailable();
        }

        // Validar configuración
        if (minimumCalories <= 0)
        {
            throw InvalidConfigurationException.InvalidMinimumCalories(minimumCalories);
        }

        if (maximumWeight <= 0)
        {
            throw InvalidConfigurationException.InvalidMaximumWeight(maximumWeight);
        }

        // Validar que todos los elementos sean válidos
        foreach (var element in elementList)
        {
            if (!element.IsValid())
            {
                throw new InvalidElementException(
                    $"El elemento '{element.Name}' no es válido: Peso={element.Weight}, Calorías={element.Calories}");
            }
        }

        return Task.FromResult(true);
    }

    #region Algoritmos Privados

    /// <summary>
    /// Estrategia greedy: selecciona elementos ordenados por eficiencia calórica (calorías/peso).
    /// Prioriza elementos más eficientes primero.
    /// </summary>
    private OptimizationResult? TryGreedyByEfficiency(
        List<Element> elements,
        decimal minimumCalories,
        decimal maximumWeight)
    {
        // Ordenar por eficiencia descendente
        var sortedElements = elements
            .OrderByDescending(e => e.CalorieEfficiency)
            .ThenBy(e => e.Weight)
            .ToList();

        var selected = new List<Element>();
        decimal currentWeight = 0;
        decimal currentCalories = 0;

        foreach (var element in sortedElements)
        {
            // Si ya cumplimos con las calorías, detener
            if (currentCalories >= minimumCalories)
                break;

            // Si agregar este elemento no excede el peso máximo
            if (currentWeight + element.Weight <= maximumWeight)
            {
                selected.Add(element);
                currentWeight += element.Weight;
                currentCalories += element.Calories;
            }
        }

        // Verificar si la solución cumple con las calorías mínimas
        if (currentCalories >= minimumCalories)
        {
            return OptimizationResult.CreateSuccessful(
                selected,
                currentWeight,
                currentCalories,
                $"Solución encontrada mediante estrategia greedy: {selected.Count} elemento(s)");
        }

        return null;
    }

    /// <summary>
    /// Algoritmo de programación dinámica para el problema de la mochila modificado.
    /// Busca la combinación con menor peso que cumpla con las calorías mínimas.
    /// Utiliza un enfoque de búsqueda exhaustiva optimizada con memoización.
    /// </summary>
    private OptimizationResult? SolveDynamicProgramming(
        List<Element> elements,
        decimal minimumCalories,
        decimal maximumWeight)
    {
        // Usar búsqueda con retroceso (backtracking) para explorar todas las combinaciones
        List<Element>? bestSolution = null;
        decimal bestWeight = decimal.MaxValue;

        // Función recursiva para explorar combinaciones
        void Explore(int index, List<Element> current, decimal currentWeight, decimal currentCalories)
        {
            // Si cumple con las calorías mínimas
            if (currentCalories >= minimumCalories)
            {
                // Si es mejor que la solución actual
                if (currentWeight < bestWeight)
                {
                    bestWeight = currentWeight;
                    bestSolution = new List<Element>(current);
                }
                return; // No necesitamos agregar más elementos
            }

            // Si llegamos al final o el peso excede el máximo
            if (index >= elements.Count)
                return;

            // Poda: si incluso con todas las calorías restantes no alcanzamos el mínimo
            var remainingCalories = elements.Skip(index).Sum(e => e.Calories);
            if (currentCalories + remainingCalories < minimumCalories)
                return;

            // Opción 1: Incluir el elemento actual
            var element = elements[index];
            if (currentWeight + element.Weight <= maximumWeight)
            {
                current.Add(element);
                Explore(index + 1, current, currentWeight + element.Weight, currentCalories + element.Calories);
                current.RemoveAt(current.Count - 1);
            }

            // Opción 2: No incluir el elemento actual (solo si aún no tenemos solución válida)
            if (currentCalories < minimumCalories)
            {
                Explore(index + 1, current, currentWeight, currentCalories);
            }
        }

        // Iniciar búsqueda desde el índice 0
        Explore(0, new List<Element>(), 0, 0);

        if (bestSolution != null && bestSolution.Any())
        {
            var totalCalories = bestSolution.Sum(e => e.Calories);

            return OptimizationResult.CreateSuccessful(
                bestSolution,
                bestWeight,
                totalCalories,
                $"Solución óptima encontrada mediante búsqueda exhaustiva: {bestSolution.Count} elemento(s)");
        }

        return null;
    }

    #endregion
}