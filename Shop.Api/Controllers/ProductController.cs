using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Application.Requests;
using Shop.Application.Responses.Base;
using Shop.Application.Responses.Product;

namespace Shop.Api.Controllers;

/// <summary>
/// Manages product operations (CRUD).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService _productService) : BaseController
{
    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> Get(CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductsAsync(cancellationToken);
        return HandleResponse(response);
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 404)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductByIdAsync(id, cancellationToken);
        return HandleResponse(response);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">Product creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 409)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateProductAsync(request, cancellationToken);
        return HandleResponse(response);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="request">Update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 404)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.UpdateProductAsync(id, request, cancellationToken);
        return HandleResponse(response);
    }

    /// <summary>
    /// Deletes a product by ID.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deletion result.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _productService.DeleteProductAsync(id, cancellationToken);
        return HandleResponse(response);
    }
}
