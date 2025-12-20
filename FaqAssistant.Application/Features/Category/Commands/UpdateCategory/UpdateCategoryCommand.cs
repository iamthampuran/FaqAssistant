using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}