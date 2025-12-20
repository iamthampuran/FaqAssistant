using FaqAssistant.Application.Features.User.Commands.DeleteUser;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _handler = new DeleteUserCommandHandler(_mockUnitOfWork.Object, _mockUserRepository.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_ValidDeletion_DeletesUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);
        var user = new Domain.Entities.User { Id = userId, IsDeleted = false };
        
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(userId, result.Data);
        Assert.True(user.IsDeleted);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFailure()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User is not authenticated.", result.Message);
    }

    [Fact]
    public async Task Handle_UnauthorizedUser_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var command = new DeleteUserCommand(otherUserId);
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("You are not authorized to delete this user's account.", result.Message);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task Handle_SetsIsDeletedFlagAndUpdatesTimestamp()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);
        var user = new Domain.Entities.User 
        { 
            Id = userId, 
            IsDeleted = false,
            LastUpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(user.IsDeleted);
        Assert.True(user.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }
}
