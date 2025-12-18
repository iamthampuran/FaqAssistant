using FaqAssistant.Domain.Entities;

namespace FaqAssistant.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    Guid? ValidateToken(string token);
}
