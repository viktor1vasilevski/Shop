using Microsoft.EntityFrameworkCore;
using Shop.Application.Constants;
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
        catch(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            if (message.Contains("IX_Products_Name", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>
                {
                    Data = null,
                    Message = ProductConstants.ProductExist,
                    Status = ResponseStatus.Conflict
                });
            }
            if (message.Contains("CK_Product_Quantity_NonNegative", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>
                {
                    Data = null,
                    Message = "Quantity cannot be negative.",
                    Status = ResponseStatus.BadRequest
                });
            }
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
