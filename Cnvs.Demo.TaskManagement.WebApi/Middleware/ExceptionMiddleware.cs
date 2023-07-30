using System.Text.Json;
using Cnvs.Demo.TaskManagement.WebApi.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace Cnvs.Demo.TaskManagement.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleSystemExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        var errors = ex.Errors.ToList();
        foreach (var error in errors)
            _logger.LogError(
                $"ValidationExtensions error: property {error.PropertyName} with value [{error.AttemptedValue}] is not valid: {error.ErrorMessage}.");

        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        var response = new
        {
            Errors = errors.Parse()
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private async Task HandleSystemExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogCritical(ex, "Unhandled exception occurred");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        const string errorMessage = "Unhandled exception occurred. Please contact support";
        var response = new
        {
            Errors = new List<ValidationFailure>
            {
                new("", errorMessage)
            }.Parse()
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}

public class Error
{
    public required string Code { get; set; }

    public required string Title { get; set; }

    public required string Detail { get; set; }
}
