using Shop.Application.Requests;
using Shop.Application.Responses.Base;
using Shop.Application.Responses.Product;

namespace Shop.Application.Interfaces;

public interface IProductService
{
    Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
}
