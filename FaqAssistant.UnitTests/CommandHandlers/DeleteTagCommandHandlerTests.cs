using FaqAssistant.Application.Features.Tag.Commands.DeleteTag;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class DeleteTagCommandHandlerTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteTagCommandHandler(_mockTagRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidDeletion_DeletesTagSuccessfully()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand { Id = tagId };
        var tag = new Domain.Entities.Tag { Id = tagId, IsDeleted = false };
        
        _mockTagRepository.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>())).ReturnsAsync(tag);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(tagId, result.Data);
        Assert.True(tag.IsDeleted);
    }

    [Fact]
    public async Task Handle_TagNotFound_ReturnsFailure()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand { Id = tagId };
        _mockTagRepository.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Tag?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Tag not found.", result.Message);
    }

    [Fact]
    public async Task Handle_AlreadyDeletedTag_ReturnsFailure()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand { Id = tagId };
        var deletedTag = new Domain.Entities.Tag { Id = tagId, IsDeleted = true };
        
        _mockTagRepository.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>())).ReturnsAsync(deletedTag);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Tag not found.", result.Message);
    }

    [Fact]
    public async Task Handle_SetsIsDeletedFlagAndUpdatesTimestamp()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand { Id = tagId };
        var tag = new Domain.Entities.Tag 
        { 
            Id = tagId, 
            IsDeleted = false,
            LastUpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        
        _mockTagRepository.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>())).ReturnsAsync(tag);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(tag.IsDeleted);
        Assert.True(tag.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand { Id = tagId };
        var tag = new Domain.Entities.Tag { Id = tagId, IsDeleted = false };
        
        _mockTagRepository.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>())).ReturnsAsync(tag);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
