using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.DeleteFaq;

public record DeleteFaqCommand(Guid Id) : IRequest<Result<Guid>>;
