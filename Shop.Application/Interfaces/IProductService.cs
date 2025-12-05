using Shop.Application.Requests;
using Shop.Application.Responses.Base;
using Shop.Application.Responses.Product;

namespace Shop.Application.Interfaces;

public interface IProductService
{
    Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
