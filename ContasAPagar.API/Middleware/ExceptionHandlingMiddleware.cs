using ContasAPagar.API.DTOs.Common;
using ContasAPagar.API.Exceptions;
using System.Net;
using System.Text.Json;

namespace ContasAPagar.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ocorreu um erro: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        ErrorResponse errorResponse;

        switch (exception)
        {
            case BusinessException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    Message = exception.Message
                };
                break;

            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new ErrorResponse
                {
                    Message = exception.Message
                };
                break;

            case MongoDB.Driver.MongoWriteException mongoEx when mongoEx.WriteError?.Code == 11000:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse = new ErrorResponse
                {
                    Message = "Já existe um registro com estes dados"
                };
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    Message = "Ocorreu um erro interno no servidor"
                };
                break;
        }

        var json = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(json);
    }
}

