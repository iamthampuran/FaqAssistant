using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.UpdateFaq;

public class UpdateFaqCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public List<Guid> TagIds { get; set; } = [];

}
