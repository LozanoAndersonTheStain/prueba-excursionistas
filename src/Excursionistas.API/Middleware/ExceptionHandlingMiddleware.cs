using Excursionistas.Application.DTOs.Response;
using Excursionistas.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Excursionistas.API.Middleware;

/// <summary>
/// Middleware para manejo centralizado de excepciones.
/// Captura todas las excepciones y las convierte en respuestas HTTP apropiadas.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Maneja la ejecución del middleware y captura excepciones.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada: {ExceptionMessage}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Procesa una excepción y genera una respuesta HTTP apropiada.
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            // Excepciones de dominio
            case InvalidElementException domainEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = domainEx.ErrorCode;
                errorResponse.Message = domainEx.Message;
                break;

            case InvalidConfigurationException configEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = configEx.ErrorCode;
                errorResponse.Message = configEx.Message;
                break;

            case NoSolutionFoundException noSolutionEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.ErrorCode = noSolutionEx.ErrorCode;
                errorResponse.Message = noSolutionEx.Message;
                break;

            case DomainException domainException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = domainException.ErrorCode;
                errorResponse.Message = domainException.Message;
                break;

            // Excepciones de validación (FluentValidation)
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "VALIDATION_ERROR";
                errorResponse.Message = "Uno o más errores de validación ocurrieron";
                errorResponse.ValidationErrors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );
                break;

            // Excepciones de argumentos
            case ArgumentNullException argNullEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "ARGUMENT_NULL";
                errorResponse.Message = $"Argumento requerido es nulo: {argNullEx.ParamName}";
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "ARGUMENT_INVALID";
                errorResponse.Message = argEx.Message;
                break;

            // Excepciones no autorizadas
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.ErrorCode = "UNAUTHORIZED";
                errorResponse.Message = "No tiene permisos para realizar esta acción";
                break;

            // Excepciones no encontradas
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.ErrorCode = "NOT_FOUND";
                errorResponse.Message = "El recurso solicitado no fue encontrado";
                break;

            // Excepciones de operación inválida
            case InvalidOperationException invalidOpEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "INVALID_OPERATION";
                errorResponse.Message = invalidOpEx.Message;
                break;

            // Excepciones generales
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                errorResponse.Message = "Ocurrió un error interno en el servidor";

                // Solo incluir stack trace en desarrollo
                if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    errorResponse.StackTrace = exception.StackTrace;
                }
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await context.Response.WriteAsync(json);
    }
}
