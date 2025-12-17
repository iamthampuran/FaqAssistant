using FaqAssistant.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

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
        byte[] saltBytes = Convert.FromBase64String(salt);
        // Hash the password with the salt using PBKDF2
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 10000, HashAlgorithmName.SHA256, 32);

        // Combine salt and hash
        byte[] hashBytes = new byte[48];
        Array.Copy(saltBytes, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        // Convert to base64 string for storage
        return Convert.ToBase64String(hashBytes);
    }
}
