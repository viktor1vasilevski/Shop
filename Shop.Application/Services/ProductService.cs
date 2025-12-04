using Shop.Application.Constants;
using Shop.Application.Enums;
using Shop.Application.Interfaces;
using Shop.Application.Requests;
using Shop.Application.Responses.Base;
using Shop.Application.Responses.Product;
using Shop.Domain.Exceptions;
using Shop.Domain.Interfaces;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class ProductService(IEfUnitOfWork _uow, IEfRepository<Product> _productRepository) : IProductService
{


    public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var trimmedName = request.Name.Trim();
        var nameTaken = await _productRepository.ExistsAsync(
            p => p.Name.ToLower() == trimmedName.ToLower(),
            cancellationToken: cancellationToken
        );

        if (nameTaken)
            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.Conflict,
                Message = ProductConstants.ProductExist
            };

        try
        {
            var product = Product.Create(request.Name, request.Description, request.Quantity);

            await _productRepository.AddAsync(product, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.Created,
                Message = ProductConstants.ProductSuccessfullyCreated,
                Data = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Created = product.Created
                }
            };
        }
        catch (DomainValidationException ex)
        {
            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.BadRequest,
                Message = ex.Message
            };
        }
    }
}
