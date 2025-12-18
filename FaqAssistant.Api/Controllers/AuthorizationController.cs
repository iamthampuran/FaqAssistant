using FaqAssistant.Application.Features.User.Commands.AuthorizeUser;
using FaqAssistant.Application.Features.User.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FaqAssistant.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthorizeUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
