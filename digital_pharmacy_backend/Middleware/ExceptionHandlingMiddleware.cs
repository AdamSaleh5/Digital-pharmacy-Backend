using Application.Constants;
using Application.Exceptions;
using Application.Models;

namespace digital_pharmacy_backend.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidSearchTypeException exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest,
                AppMessages.InvalidSearchType, exception, LogLevel.Warning);
        }
        catch (KeyNotFoundException exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound,
                AppMessages.MedicationNotFound, exception, LogLevel.Warning);
        }
        catch (UnauthorizedAccessException exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden,
                AppMessages.UnauthorizedAccess, exception, LogLevel.Warning);
        }
        catch (ArgumentException exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest,
                exception.Message, exception, LogLevel.Warning);
        }
        catch (Exception exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                AppMessages.UnexpectedError, exception, LogLevel.Error);
        }
    }

    private async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        Exception exception,
        LogLevel logLevel)
    {
        logger.Log(logLevel, exception, "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method, context.Request.Path, context.TraceIdentifier);

        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Failure(statusCode, message));
    }
}
