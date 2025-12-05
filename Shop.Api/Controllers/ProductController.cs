using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Application.Requests;
using Shop.Application.Responses.Base;
using Shop.Application.Responses.Product;

namespace Shop.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService _productService) : BaseController
{

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> Get(CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductsAsync(cancellationToken);
        return HandleResponse(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 404)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductByIdAsync(id, cancellationToken);
        return HandleResponse(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 409)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateProductAsync(request, cancellationToken);
        return HandleResponse(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 404)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 409)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.UpdateProductAsync(id, request, cancellationToken);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _productService.DeleteProductAsync(id, cancellationToken);
        return HandleResponse(response);
    }
}
