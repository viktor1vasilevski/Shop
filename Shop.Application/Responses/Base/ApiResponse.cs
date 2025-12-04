using Shop.Application.Enums;

namespace Shop.Application.Responses.Base;

public class ApiResponse<T> where T : class
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ResponseStatus Status { get; set; }
}
