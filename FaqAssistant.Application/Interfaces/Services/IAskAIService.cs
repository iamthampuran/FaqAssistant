namespace FaqAssistant.Application.Interfaces.Services;

public interface IAskAIService
{
    Task<string?> GetAnswerAsync(Guid faqId, CancellationToken cancellationToken);
}
