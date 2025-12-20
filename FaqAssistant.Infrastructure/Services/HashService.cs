using FaqAssistant.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace FaqAssistant.Infrastructure.Services;

public class HashService : IHashService
{
    private readonly IConfiguration _configuration;
    public HashService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    public string HashPassword(string password)
    {
        var salt = _configuration["Security:PasswordSalt"];
        if (string.IsNullOrEmpty(salt))
        {
            throw new InvalidOperationException("Password salt is not configured.");
        }
        return BCrypt.Net.BCrypt.HashPassword(password + salt);
    }

    public bool VerifyHashPassword(string password, string hash)
    {
        var salt = _configuration["Security:PasswordSalt"];
        if (string.IsNullOrEmpty(salt))
        {
            throw new InvalidOperationException("Password salt is not configured.");
        }
        Console.WriteLine($"Received password - {password}, it's hash value - {HashPassword(password)} - actual hash - {hash}");

        return BCrypt.Net.BCrypt.Verify(password + salt, hash);
    }
}
