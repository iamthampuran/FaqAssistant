using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Commands.UpdateTag;

public class UpdateTagCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
