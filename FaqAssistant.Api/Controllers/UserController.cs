using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.User.Commands.DeleteUser;
using FaqAssistant.Application.Features.User.Commands.UpdateUser;
using FaqAssistant.Application.Features.User.Queries.GetUserDetails;
using FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;
using FaqAssistant.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaqAssistant.Api.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id)
    {
        var query = new GetUserDetailsByIdQuery(id);
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        return NotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserDetails([FromQuery] string? searchValue, [FromQuery] PageParameters pageParameters)
    {
        var query = new GetUserDetailsQuery(pageParameters.PageSize, pageParameters.PageNumber, searchValue);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new Result<Guid>(false, "User ID mismatch."));
        }
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        // Simulate delete logic
        // In a real implementation, you would send a command to delete the user
        var query = new DeleteUserCommand(id);
        var result = await _mediator.Send(query);
        return result.Success ? Ok(new Result<Guid>(true, id)) : BadRequest(result);
    }
}

