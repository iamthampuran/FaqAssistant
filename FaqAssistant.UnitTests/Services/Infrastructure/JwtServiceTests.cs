using FaqAssistant.Domain.Entities;
using FaqAssistant.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FaqAssistant.UnitTests.Services.Infrastructure;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly JwtService _jwtService;
    private const string SecretKey = "this_is_a_very_long_secret_key_for_testing_purposes_at_least_32_characters";
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";
    private const string ExpirationMinutes = "60";

    public JwtServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"]).Returns(SecretKey);
        _mockConfiguration.Setup(x => x["JwtSettings:Issuer"]).Returns(Issuer);
        _mockConfiguration.Setup(x => x["JwtSettings:Audience"]).Returns(Audience);
        _mockConfiguration.Setup(x => x["JwtSettings:ExpirationMinutes"]).Returns(ExpirationMinutes);
        _jwtService = new JwtService(_mockConfiguration.Object);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@example.com", 
            Username = "testuser"
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateToken_CreatesValidJwtToken()
    {
        // Arrange
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@example.com", 
            Username = "testuser"
        };

        // Act
        var token = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.NotNull(jwtToken);
        Assert.Equal(Issuer, jwtToken.Issuer);
        Assert.Contains(Audience, jwtToken.Audiences);
    }

    [Fact]
    public void GenerateToken_IncludesUserClaims()
    {
        // Arrange
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@example.com", 
            Username = "testuser"
        };

        // Act
        var token = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "username");

        Assert.NotNull(subClaim);
        Assert.Equal(user.Id.ToString(), subClaim.Value);
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
        Assert.NotNull(usernameClaim);
        Assert.Equal(user.Username, usernameClaim.Value);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsUserId()
    {
        // Arrange
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@example.com", 
            Username = "testuser"
        };
        var token = _jwtService.GenerateToken(user);

        // Act
        var result = _jwtService.ValidateToken(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Value);
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ReturnsNull()
    {
        // Act
        var result = _jwtService.ValidateToken("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ValidateToken_WithNullToken_ReturnsNull()
    {
        // Act
        var result = _jwtService.ValidateToken(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        // Act
        var result = _jwtService.ValidateToken("invalid.token.here");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ReturnsNull()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["JwtSettings:SecretKey"]).Returns(SecretKey);
        mockConfig.Setup(x => x["JwtSettings:Issuer"]).Returns(Issuer);
        mockConfig.Setup(x => x["JwtSettings:Audience"]).Returns(Audience);
        mockConfig.Setup(x => x["JwtSettings:ExpirationMinutes"]).Returns("-1"); // Expired
        var expiredJwtService = new JwtService(mockConfig.Object);

        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@example.com", 
            Username = "testuser"
        };
        var token = expiredJwtService.GenerateToken(user);
        System.Threading.Thread.Sleep(1000); // Wait to ensure token is expired

        // Act
        var result = _jwtService.ValidateToken(token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GenerateToken_DifferentUsers_ProduceDifferentTokens()
    {
        // Arrange
        var user1 = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test1@example.com", 
            Username = "user1"
        };
        var user2 = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test2@example.com", 
            Username = "user2"
        };

        // Act
        var token1 = _jwtService.GenerateToken(user1);
        var token2 = _jwtService.GenerateToken(user2);

        // Assert
        Assert.NotEqual(token1, token2);
        Assert.Equal(user1.Id, _jwtService.ValidateToken(token1));
        Assert.Equal(user2.Id, _jwtService.ValidateToken(token2));
    }
}
