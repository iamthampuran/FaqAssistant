using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Services;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.AskAI;

public class AskAICommandHandler : IRequestHandler<AskAICommand, Result<string>>
{
    private readonly IAskAIService _askAIService;
    public AskAICommandHandler(IAskAIService askAIService)
    {
        _askAIService = askAIService ?? throw new ArgumentNullException(nameof(askAIService));
    }

    public async Task<Result<string>> Handle(AskAICommand request, CancellationToken cancellationToken)
    {
        var answer = await _askAIService.GetAnswerAsync(request.FaqId, cancellationToken);
        if (answer == null)
        {
            return new Result<string>(false, "FAQ not found or AI service failed to provide an answer.");
        }
        return new Result<string>(true, answer, null);
    }
}
