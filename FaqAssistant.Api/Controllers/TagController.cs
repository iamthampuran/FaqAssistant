using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Tag.Commands.CreateTag;
using FaqAssistant.Application.Features.Tag.Commands.DeleteTag;
using FaqAssistant.Application.Features.Tag.Commands.UpdateTag;
using FaqAssistant.Application.Features.Tag.Queries.GetAllTags;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetailsById;
using FaqAssistant.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaqAssistant.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTags()
    {
        var query = new GetAllTagsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetTagDetails([FromQuery] string? searchValue, [FromQuery] PageParameters pageParameters)
    {
        var query = new GetTagDetailsQuery(pageParameters.PageSize, pageParameters.PageNumber, searchValue);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateTag([FromRoute] Guid id, [FromBody] UpdateTagCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new Result<Guid>(false, "Tag ID mismatch."));
        }
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteTag([FromRoute] Guid id)
    {
        var command = new DeleteTagCommand { Id = id };
        var result = await _mediator.Send(command);
        return result.Success ? Ok(new Result<Guid>(true, id)) : BadRequest(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return CreatedAtAction(nameof(CreateTag), new { id = result.Data }, result);
        }
        return BadRequest(result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById([FromRoute] Guid id)
    {
        var query = new GetTagDetailsByIdQuery(id);
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        return NotFound(result);
    }
}