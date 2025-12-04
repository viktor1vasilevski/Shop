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


    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateProductAsync(request, cancellationToken);
        return HandleResponse(response);
    }
}
