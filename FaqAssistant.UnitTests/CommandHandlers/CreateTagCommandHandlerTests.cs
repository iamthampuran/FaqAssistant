using FaqAssistant.Application.Features.Tag.Commands.CreateTag;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class CreateTagCommandHandlerTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateTagCommandHandler _handler;

    public CreateTagCommandHandlerTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateTagCommandHandler(_mockTagRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidTagName_CreatesTagSuccessfully()
    {
        // Arrange
        var command = new CreateTagCommand { Name = "CSharp" };
        _mockTagRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Tag>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        _mockTagRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Tag>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreatesTag_WithCorrectName()
    {
        // Arrange
        var tagName = "JavaScript";
        var command = new CreateTagCommand { Name = tagName };
        Domain.Entities.Tag? capturedTag = null;
        
        _mockTagRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Tag>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Tag, CancellationToken>((tag, ct) => capturedTag = tag)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(capturedTag);
        Assert.Equal(tagName, capturedTag.Name);
    }

    [Fact]
    public async Task Handle_CreatesTag_WithCurrentTimestamps()
    {
        // Arrange
        var command = new CreateTagCommand { Name = "Python" };
        Domain.Entities.Tag? capturedTag = null;
        
        _mockTagRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Tag>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Tag, CancellationToken>((tag, ct) => capturedTag = tag)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var beforeCreation = DateTime.UtcNow;
        await _handler.Handle(command, CancellationToken.None);
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.NotNull(capturedTag);
        Assert.True(capturedTag.CreatedAt >= beforeCreation && capturedTag.CreatedAt <= afterCreation);
        Assert.True(capturedTag.LastUpdatedAt >= beforeCreation && capturedTag.LastUpdatedAt <= afterCreation);
        Assert.False(capturedTag.IsDeleted);
    }

    [Fact]
    public async Task Handle_GeneratesUniqueIds()
    {
        // Arrange
        var command1 = new CreateTagCommand { Name = "Tag1" };
        var command2 = new CreateTagCommand { Name = "Tag2" };
        var capturedIds = new List<Guid>();
        
        _mockTagRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Tag>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Tag, CancellationToken>((tag, ct) => capturedIds.Add(tag.Id))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.Data, result2.Data);
    }
}
