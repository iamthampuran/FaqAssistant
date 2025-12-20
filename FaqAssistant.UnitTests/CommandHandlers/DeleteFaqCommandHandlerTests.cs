using FaqAssistant.Application.Features.Faq.Commands.DeleteFaq;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class DeleteFaqCommandHandlerTests
{
    private readonly Mock<IFaqRepository> _mockFaqRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly DeleteFaqCommandHandler _handler;
    public DeleteFaqCommandHandlerTests()
    {
        _mockFaqRepository = new Mock<IFaqRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _handler = new DeleteFaqCommandHandler(_mockFaqRepository.Object, _mockUnitOfWork.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_FaqExists_DeletesFaqSuccessfully()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteFaqCommand(faqId);
        var faq = new Faq
        {
            Id = faqId,
            UserId = userId,
            IsDeleted = false,
            Tags = new List<FaqTag>
            {
                new FaqTag { Id = Guid.NewGuid(), IsDeleted = false },
                new FaqTag { Id = Guid.NewGuid(), IsDeleted = false }
            }
        };
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<Expression<Func<Faq, bool>>>(),
            It.IsAny<List<Expression<Func<Faq, object>>>>(),
            false,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.Success);
        Assert.Equal(faqId, result.Data);
        Assert.Equal("Faq deleted successfully.", result.Message);
        Assert.True(faq.IsDeleted);
        foreach (var tag in faq.Tags)
        {
            Assert.True(tag.IsDeleted);
        }
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FaqDoesNotExist_ReturnsNotFoundMessage()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteFaqCommand(faqId);
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<Expression<Func<Faq, bool>>>(),
            It.IsAny<List<Expression<Func<Faq, object>>>>(),
            false,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Faq?)null);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.Success);
        Assert.Equal("Faq not found.", result.Message);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
