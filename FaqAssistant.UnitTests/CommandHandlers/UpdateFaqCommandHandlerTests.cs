using FaqAssistant.Application.Features.Faq.Commands.UpdateFaq;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Domain.Entities;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class UpdateFaqCommandHandlerTests
{
    private readonly Mock<IFaqRepository> _mockFaqRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly UpdateFaqCommandHandler _handler;

    public UpdateFaqCommandHandlerTests()
    {
        _mockFaqRepository = new Mock<IFaqRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _handler = new UpdateFaqCommandHandler(_mockFaqRepository.Object, _mockUnitOfWork.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_WhenFaqNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateFaqCommand
        {
            Id = faqId,
            Question = "Updated Question",
            Answer = "Updated Answer",
            UserId = userId,
            CategoryId = Guid.NewGuid(),
            TagIds = [Guid.NewGuid()]
        };

        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Faq, bool>>>(),
            It.IsAny<List<System.Linq.Expressions.Expression<Func<Faq, object>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Faq?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Faq was not found", result.Message);
        Assert.Equal(Guid.Empty, result.Data);
    }

    [Fact]
    public async Task Handle_WhenFaqIsDeleted_ShouldReturnFailureResult()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateFaqCommand
        {
            Id = faqId,
            Question = "Updated Question",
            Answer = "Updated Answer",
            UserId = userId,
            CategoryId = Guid.NewGuid(),
            TagIds = []
        };

        var deletedFaq = new Faq
        {
            Id = faqId,
            Question = "Original",
            Answer = "Original Answer",
            UserId = userId,
            CategoryId = Guid.NewGuid(),
            IsDeleted = true,
            Tags = []
        };

        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Faq, bool>>>(),
            It.IsAny<List<System.Linq.Expressions.Expression<Func<Faq, object>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedFaq);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Faq was not found", result.Message);
    }

    [Fact]
    public async Task Handle_WhenNoTagsSent_ShouldSoftDeleteAllFaqTags()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var tagId1 = Guid.NewGuid();
        var tagId2 = Guid.NewGuid();

        var command = new UpdateFaqCommand
        {
            Id = faqId,
            Question = "Updated Question",
            Answer = "Updated Answer",
            UserId = userId,
            CategoryId = categoryId,
            TagIds = []
        };

        var faq = new Faq
        {
            Id = faqId,
            Question = "Original",
            Answer = "Original Answer",
            UserId = userId,
            CategoryId = categoryId,
            IsDeleted = false,
            Tags = new List<FaqTag>
            {
                new() { Id = Guid.NewGuid(), FaqId = faqId, TagId = tagId1, IsDeleted = false },
                new() { Id = Guid.NewGuid(), FaqId = faqId, TagId = tagId2, IsDeleted = false }
            }
        };

        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Faq, bool>>>(),
            It.IsAny<List<System.Linq.Expressions.Expression<Func<Faq, object>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(faqId, result.Data);
        Assert.Equal("Faq updated successfully", result.Message);

        // Verify all tags were marked as deleted
        foreach (var tag in faq.Tags)
        {
            Assert.True(tag.IsDeleted);
        }

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUpdatingWithNewTags_ShouldHandleTagChanges()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var tag1 = Guid.NewGuid();
        var tag2 = Guid.NewGuid();
        var tag3 = Guid.NewGuid();

        var command = new UpdateFaqCommand
        {
            Id = faqId,
            Question = "Updated Question",
            Answer = "Updated Answer",
            UserId = userId,
            CategoryId = categoryId,
            TagIds = [tag1, tag3]
        };

        var faq = new Faq
        {
            Id = faqId,
            Question = "Original",
            Answer = "Original Answer",
            UserId = userId,
            CategoryId = categoryId,
            IsDeleted = false,
            Tags = new List<FaqTag>
            {
                new() { Id = Guid.NewGuid(), FaqId = faqId, TagId = tag1, IsDeleted = false },
                new() { Id = Guid.NewGuid(), FaqId = faqId, TagId = tag2, IsDeleted = false }
            }
        };

        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Faq, bool>>>(),
            It.IsAny<List<System.Linq.Expressions.Expression<Func<Faq, object>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);

        _mockFaqRepository.Setup(r => r.AddNewTags(It.IsAny<List<Guid>>(), faqId))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(faqId, result.Data);
        Assert.Equal("Faq updated successfully", result.Message);

        // Verify tag2 was soft deleted (not in incoming tags)
        var tag2Item = faq.Tags.First(t => t.TagId == tag2);
        Assert.True(tag2Item.IsDeleted);

        // Verify tag1 remains active
        var tag1Item = faq.Tags.First(t => t.TagId == tag1);
        Assert.False(tag1Item.IsDeleted);

        // Verify new tags were added
        _mockFaqRepository.Verify(r => r.AddNewTags(
            It.Is<List<Guid>>(list => list.Count == 1 && list.Contains(tag3)),
            faqId),
            Times.Once);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReactivatingDeletedTags_ShouldMarkAsNotDeleted()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var tag1 = Guid.NewGuid();

        var command = new UpdateFaqCommand
        {
            Id = faqId,
            Question = "Updated Question",
            Answer = "Updated Answer",
            UserId = userId,
            CategoryId = categoryId,
            TagIds = [tag1]
        };

        var faq = new Faq
        {
            Id = faqId,
            Question = "Original",
            Answer = "Original Answer",
            UserId = userId,
            CategoryId = categoryId,
            IsDeleted = false,
            Tags = new List<FaqTag>
            {
                new() { Id = Guid.NewGuid(), FaqId = faqId, TagId = tag1, IsDeleted = true }
            }
        };

        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Faq, bool>>>(),
            It.IsAny<List<System.Linq.Expressions.Expression<Func<Faq, object>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(faqId, result.Data);

        // Verify deleted tag was reactivated
        var tag1Item = faq.Tags.First(t => t.TagId == tag1);
        Assert.False(tag1Item.IsDeleted);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateBasicProperties()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var oldCategoryId = Guid.NewGuid();
        const string newQuestion = "New Question";
        const string newAnswer = "New Answer";

        var command = new UpdateFaqCommand
        {
            Id = faqId,
            Question = newQuestion,
            Answer = newAnswer,
            UserId = userId,
            CategoryId = newCategoryId,
            TagIds = []
        };

        var faq = new Faq
        {
            Id = faqId,
            Question = "Old Question",
            Answer = "Old Answer",
            UserId = userId,
            CategoryId = oldCategoryId,
            IsDeleted = false,
            Tags = []
        };

        var originalLastUpdated = faq.LastUpdatedAt;

        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockFaqRepository.Setup(r => r.GetFirstAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Faq, bool>>>(),
            It.IsAny<List<System.Linq.Expressions.Expression<Func<Faq, object>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(newQuestion, faq.Question);
        Assert.Equal(newAnswer, faq.Answer);
        Assert.Equal(newCategoryId, faq.CategoryId);
        Assert.True(faq.LastUpdatedAt > originalLastUpdated);
    }
}
