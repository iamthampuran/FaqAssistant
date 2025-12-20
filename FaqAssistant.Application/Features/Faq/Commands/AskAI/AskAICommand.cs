using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.AskAI;

public record AskAICommand(Guid FaqId) : IRequest<Result<string>>;

