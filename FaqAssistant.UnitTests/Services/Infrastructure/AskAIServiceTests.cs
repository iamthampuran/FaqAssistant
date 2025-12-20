using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using FaqAssistant.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;

namespace FaqAssistant.UnitTests.Services.Infrastructure;

public class AskAIServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IFaqRepository> _mockFaqRepository;
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly AskAIService _askAIService;
    private const string ApiKey = "test_api_key";
    private const string BaseUrl = "https://api.example.com/ask";

    public AskAIServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(x => x["AISettings:Key"]).Returns(ApiKey);
        _mockConfiguration.Setup(x => x["AISettings:BaseUrl"]).Returns(BaseUrl);
        
        _mockFaqRepository = new Mock<IFaqRepository>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        
        // Setup default cache behavior - cache miss
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);
        
        _askAIService = new AskAIService(
            _mockConfiguration.Object,
            _mockFaqRepository.Object,
            new HttpClient(),
            _mockMemoryCache.Object
        );
    }

    [Fact]
    public async Task GetAnswerAsync_WithFaqNotFound_ReturnsNull()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        _mockFaqRepository.Setup(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Faq?)null);

        // Act
        var result = await _askAIService.GetAnswerAsync(faqId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAnswerAsync_WithCachedAnswer_ReturnsCachedValue()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var cachedAnswer = "This is a cached answer";
        object? cachedObject = cachedAnswer;
        
        _mockMemoryCache
            .Setup(x => x.TryGetValue(faqId, out cachedObject))
            .Returns(true);

        // Act
        var result = await _askAIService.GetAnswerAsync(faqId, CancellationToken.None);

        // Assert
        Assert.Equal(cachedAnswer, result);
        _mockFaqRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAnswerAsync_WithoutApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var faq = new Faq { Id = faqId, Question = "What is C#?" };
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["AISettings:Key"]).Returns((string?)null);
        mockConfig.Setup(x => x["AISettings:BaseUrl"]).Returns(BaseUrl);
        
        var mockMemoryCache = new Mock<IMemoryCache>();
        mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);
        
        var service = new AskAIService(
            mockConfig.Object,
            _mockFaqRepository.Object,
            new HttpClient(),
            mockMemoryCache.Object
        );

        _mockFaqRepository.Setup(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.GetAnswerAsync(faqId, CancellationToken.None));
    }

    [Fact]
    public async Task GetAnswerAsync_VerifiesFaqRepositoryCall()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        _mockFaqRepository.Setup(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Faq?)null);

        // Act
        await _askAIService.GetAnswerAsync(faqId, CancellationToken.None);

        // Assert
        _mockFaqRepository.Verify(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAnswerAsync_WithNullCachedAnswer_FetchesFromRepository()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        _mockFaqRepository.Setup(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Faq { Id = faqId, Question = "Test?" });

        // Act
        await _askAIService.GetAnswerAsync(faqId, CancellationToken.None);

        // Assert
        _mockFaqRepository.Verify(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAnswerAsync_WithHttpException_ReturnsNull()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var faq = new Faq { Id = faqId, Question = "What is C#?" };
        _mockFaqRepository.Setup(r => r.GetByIdAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);

        // Act - This will fail because HttpClient throws when calling invalid endpoint,
        // and AskAIService catches the exception and returns null
        var result = await _askAIService.GetAnswerAsync(faqId, CancellationToken.None);

        // Assert - Should return null due to exception handling
        Assert.Null(result);
    }
}
