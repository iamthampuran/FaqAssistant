using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Commands.DeleteTag;

public class DeleteTagCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
}
