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

    public async Task<ApiResponse<List<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(asNoTracking: true, cancellationToken: cancellationToken);

        return new ApiResponse<List<ProductResponseDto>>
        {
            Status = ResponseStatus.Success,
            Data = (products ?? Enumerable.Empty<Product>())
                   .Select(x => new ProductResponseDto
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Description = x.Description,
                       Quantity = x.Quantity,
                       Created = x.Created,
                       LastModified = x.LastModified,
                   })
                   .ToList()
        };
    }

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

    public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, asNoTracking: true, cancellationToken: cancellationToken);

        if (product is null)
            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.NotFound,
                Message = ProductConstants.ProductWithIdNotFound
            };

        return new ApiResponse<ProductResponseDto>
        {
            Status = ResponseStatus.Success,
            Data = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Created = product.Created,
                LastModified = product.LastModified
            }
        };
    }

    public async Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken: cancellationToken);

        if (product is null)
            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.NotFound,
                Message = ProductConstants.ProductWithIdNotFound
            };

        bool nameExists = await _productRepository.ExistsAsync(
            p => p.Name.ToLower() == request.Name.Trim().ToLower() && p.Id != id,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (nameExists)
            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.Conflict,
                Message = ProductConstants.ProductExist
            };

        try
        {
            product.Update(request.Name, request.Description, request.Quantity);

            await _uow.SaveChangesAsync(cancellationToken);

            return new ApiResponse<ProductResponseDto>
            {
                Status = ResponseStatus.Updated,
                Message = ProductConstants.ProductSuccessfullyUpdated,
                Data = new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Created = product.Created,
                    LastModified = product.LastModified
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

    public async Task<ApiResponse<object>> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken: cancellationToken);

        if (product is null)
            return new ApiResponse<object>
            {
                Status = ResponseStatus.NotFound,
                Message = ProductConstants.ProductWithIdNotFound
            };

        _productRepository.Delete(product);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ApiResponse<object>
        {
            Status = ResponseStatus.Success,
            Message = ProductConstants.ProductSuccessfullyDeleted,
        };
    }

}
