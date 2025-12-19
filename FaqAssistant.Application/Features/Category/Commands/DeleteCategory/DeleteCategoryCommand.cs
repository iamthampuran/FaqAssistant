using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
}