using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.CreateFaq;

public class CreateFaqCommand : IRequest<Result<Guid>>
{
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public List<Guid> TagIds { get; set; } = [];
}