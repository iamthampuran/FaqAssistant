using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Category.Commands.CreateCategory;
using FaqAssistant.Application.Features.Category.Commands.DeleteCategory;
using FaqAssistant.Application.Features.Category.Commands.UpdateCategory;
using FaqAssistant.Application.Features.Category.Queries.GetAllCategories;
using FaqAssistant.Application.Features.Category.Queries.GetCategoryById;
using FaqAssistant.Application.Features.Category.Queries.GetCategoryDetails;
using FaqAssistant.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaqAssistant.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return CreatedAtAction(nameof(CreateCategory), result.Data);
        }
        return BadRequest(result);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new Result<Guid>(false, "Category ID mismatch."));
        }
        var result = await _mediator.Send(command);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await _mediator.Send(command);
        return result.Success ? Ok(new Result<Guid>(true, id)) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        return NotFound(result);
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetCategoryDetails([FromQuery] string? searchValue, [FromQuery] PageParameters pageParameters)
    {
        var query = new GetCategoryDetailsQuery(pageParameters.PageSize, pageParameters.PageNumber, searchValue);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}