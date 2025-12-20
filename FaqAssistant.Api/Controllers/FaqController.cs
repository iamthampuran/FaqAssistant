using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Faq.Commands.CreateFaq;
using FaqAssistant.Application.Features.Faq.Commands.DeleteFaq;
using FaqAssistant.Application.Features.Faq.Commands.UpdateFaq;
using FaqAssistant.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaqAssistant.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FaqController : ControllerBase
{
    private readonly IMediator _mediator;

    public FaqController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateFaq([FromBody] CreateFaqCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return CreatedAtAction(nameof(CreateFaq), new { id = result.Data }, result);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFaq([FromRoute] Guid id, [FromBody] UpdateFaqCommand command)
    {
        if (id != command.Id)
            return BadRequest("UserId mismatch");
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result.Data) : NotFound(result.Message); 
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFaq([FromRoute] Guid id)
    {
        var command = new DeleteFaqCommand(id);
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetFaqDetails([FromQuery] Guid? categoryId, [FromQuery] Guid? tagId, [FromQuery] PageParameters pageParameters)
    {
        var query = new Application.Features.Faq.Queries.GetFaqDetails.GetFaqDetailsQuery(pageParameters, categoryId, tagId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("askai/{faqId}")]
    public async Task<IActionResult> AskAI([FromRoute] Guid faqId)
    {
        var command = new Application.Features.Faq.Commands.AskAI.AskAICommand(faqId);
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result.Data) : BadRequest(result.Message);
    }
}