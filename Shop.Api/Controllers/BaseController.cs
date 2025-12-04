
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Enums;
using Shop.Application.Responses.Base;

namespace Shop.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected ActionResult HandleResponse<T>(ApiResponse<T> response) where T : class
    {
        return response.Status switch
        {
            ResponseStatus.Success => Ok(response),
            ResponseStatus.BadRequest => BadRequest(response),
            ResponseStatus.NotFound => NotFound(response),
            ResponseStatus.Created => StatusCode(201, response),
            ResponseStatus.NoContent => NoContent(),
            ResponseStatus.Error => StatusCode(500, response),
            ResponseStatus.Conflict => StatusCode(409, response),
            _ => Ok(response),
        };
    }
}
