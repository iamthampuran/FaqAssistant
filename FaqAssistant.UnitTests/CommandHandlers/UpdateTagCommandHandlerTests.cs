using FaqAssistant.Application.Features.Tag.Commands.UpdateTag;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class UpdateTagCommandHandlerTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateTagCommandHandler(_mockTagRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesTagSuccessfully()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand { Id = tagId, Name = "UpdatedTag" };
        var tag = new Domain.Entities.Tag { Id = tagId, Name = "OldTag" };
        
        _mockTagRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(tagId, result.Data);
        Assert.Equal("UpdatedTag", tag.Name);
    }

    [Fact]
    public async Task Handle_TagNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateTagCommand { Id = Guid.NewGuid(), Name = "NewName" };
        _mockTagRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Tag?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Tag not found.", result.Message);
    }

    [Fact]
    public async Task Handle_DuplicateTagName_ReturnsFailure()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var otherTagId = Guid.NewGuid();
        var command = new UpdateTagCommand { Id = tagId, Name = "DuplicateName" };
        var otherTag = new Domain.Entities.Tag { Id = otherTagId, Name = "DuplicateName" };
        
        _mockTagRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherTag);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("DuplicateName", result.Message);
        Assert.Contains("already exists", result.Message);
    }

    [Fact]
    public async Task Handle_DeletedTag_ReturnsFailure()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand { Id = tagId, Name = "NewName" };
        var deletedTag = new Domain.Entities.Tag { Id = tagId, IsDeleted = true };
        
        _mockTagRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedTag);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Tag not found.", result.Message);
    }

    [Fact]
    public async Task Handle_UpdatesLastUpdatedAtTimestamp()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand { Id = tagId, Name = "Updated" };
        var tag = new Domain.Entities.Tag { Id = tagId, Name = "Old", LastUpdatedAt = DateTime.UtcNow.AddHours(-1) };
        
        _mockTagRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(tag.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }
}
