using CampusERP.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected OkObjectResult Success<T>(T data)
    {
        return Ok(ApiResponse<T>.SuccessResponse(data));
    }

    protected OkObjectResult Success<T>(T data, string message)
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }
}