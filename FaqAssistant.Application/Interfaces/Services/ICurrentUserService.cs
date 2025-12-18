namespace FaqAssistant.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();
    bool IsAuthenticated();
}
