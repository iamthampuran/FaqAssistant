using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = null!;
}