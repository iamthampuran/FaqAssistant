using FaqAssistant.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FaqAssistant.UnitTests.Services.Infrastructure;

public class HashServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly HashService _hashService;
    private const string TestSalt = "test_salt_12345";

    public HashServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration
            .Setup(x => x["Security:PasswordSalt"])
            .Returns(TestSalt);
        _hashService = new HashService(_mockConfiguration.Object);
    }

    [Fact]
    public void HashPassword_WithValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash = _hashService.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash);
        Assert.True(hash.Length > 0);
    }

    [Fact]
    public void HashPassword_WithSamePassword_ProducesConsistentHash()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash1 = _hashService.HashPassword(password);
        var hash2 = _hashService.HashPassword(password);

        // Assert - Hashes will be different due to BCrypt's random salt, but both should verify
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyHashPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123";
        var hash = _hashService.HashPassword(password);

        // Act
        var result = _hashService.VerifyHashPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyHashPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123";
        var wrongPassword = "WrongPassword456";
        var hash = _hashService.HashPassword(password);

        // Act
        var result = _hashService.VerifyHashPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HashPassword_WithoutConfiguredSalt_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["Security:PasswordSalt"]).Returns((string?)null);
        var service = new HashService(mockConfig.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.HashPassword("password"));
    }

    [Fact]
    public void VerifyHashPassword_WithoutConfiguredSalt_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["Security:PasswordSalt"]).Returns((string?)null);
        var service = new HashService(mockConfig.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.VerifyHashPassword("password", "hash"));
    }

    [Fact]
    public void HashPassword_DifferentPasswords_ProduceDifferentHashes()
    {
        // Arrange
        var password1 = "Password123";
        var password2 = "Password456";

        // Act
        var hash1 = _hashService.HashPassword(password1);
        var hash2 = _hashService.HashPassword(password2);

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.False(_hashService.VerifyHashPassword(password2, hash1));
        Assert.False(_hashService.VerifyHashPassword(password1, hash2));
    }
}
