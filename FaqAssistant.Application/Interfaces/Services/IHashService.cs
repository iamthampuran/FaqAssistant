namespace FaqAssistant.Application.Interfaces.Services;

public interface IHashService
{
    string HashPassword(string password);
    bool VerifyHashPassword(string password, string hash);

}
