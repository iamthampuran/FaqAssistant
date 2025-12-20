using FaqAssistant.Application.Features.User.Commands.UpdateUser;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _handler = new UpdateUserCommandHandler(_mockUserRepository.Object, _mockUnitOfWork.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand { Id = userId, UserName = "updated_user", Email = "updated@example.com" };
        var user = new Domain.Entities.User { Id = userId, Username = "old_user", Email = "old@example.com" };
        
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(userId, result.Data);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateUserCommand { Id = Guid.NewGuid(), UserName = "user", Email = "user@example.com" };
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
        var command = new UpdateUserCommand { Id = otherUserId, UserName = "user", Email = "user@example.com" };
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("You are not authorized to update this user's details.", result.Message);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand { Id = userId, UserName = "user", Email = "user@example.com" };
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var command = new UpdateUserCommand { Id = userId, UserName = "user", Email = "duplicate@example.com" };
        var otherUser = new Domain.Entities.User { Id = otherUserId, Email = "duplicate@example.com" };
        
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("A user with the same email or username already exists.", result.Message);
    }

    [Fact]
    public async Task Handle_PartialUpdate_UpdatesOnlyProvidedFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand { Id = userId, UserName = "updated_user", Email = null };
        var user = new Domain.Entities.User { Id = userId, Username = "old_user", Email = "old@example.com" };
        
        _mockCurrentUserService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("updated_user", user.Username);
        Assert.Equal("old@example.com", user.Email);
    }
}
