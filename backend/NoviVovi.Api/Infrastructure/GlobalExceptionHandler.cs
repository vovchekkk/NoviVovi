using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Domain.Common;

namespace NoviVovi.Api.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Произошла необработанная ошибка: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Ресурс не найден"),
            BadRequestException => (HttpStatusCode.BadRequest, "Некорректный запрос"),
            ConflictException => (HttpStatusCode.Conflict, "Конфликт состояния"),
            DomainException => (HttpStatusCode.UnprocessableEntity, "Ошибка бизнес-логики"),
            _ => (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера")
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}