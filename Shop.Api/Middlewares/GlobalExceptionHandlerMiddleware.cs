using Shop.Application.Enums;
using Shop.Application.Responses.Base;

namespace Shop.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate _next, ILogger<GlobalExceptionHandlerMiddleware> _logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            var response = new ApiResponse<object>
            {
                Data = null,
                Message = "An unexpected error occurred.",
                Status = ResponseStatus.Error,
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
