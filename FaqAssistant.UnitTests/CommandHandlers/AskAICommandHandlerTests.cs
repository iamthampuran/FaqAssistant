using FaqAssistant.Application.Features.Faq.Commands.AskAI;
using FaqAssistant.Application.Interfaces.Services;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class AskAICommandHandlerTests
{
    private readonly Mock<IAskAIService> _mockAIService;
    private readonly AskAICommandHandler _handler;

    public AskAICommandHandlerTests()
    {
        _mockAIService = new Mock<IAskAIService>();
        _handler = new AskAICommandHandler(_mockAIService.Object);
    }

    [Fact]
    public async Task Handle_FaqExists_ReturnsAnswerSuccessfully()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var command = new AskAICommand(faqId);
        var expectedAnswer = "This is a sample answer from AI.";
        _mockAIService.Setup(s => s.GetAnswerAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAnswer);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.Success);
        Assert.Equal(expectedAnswer, result.Data);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task Handle_FaqDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var command = new AskAICommand(faqId);
        _mockAIService.Setup(s => s.GetAnswerAsync(faqId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("FAQ not found or AI service failed to provide an answer.", result.Message);
    }
}
