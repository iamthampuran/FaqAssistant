using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Commands.CreateTag;

public class CreateTagCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = null!;
}
