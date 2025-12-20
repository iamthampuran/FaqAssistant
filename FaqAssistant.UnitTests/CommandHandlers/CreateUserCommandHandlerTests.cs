using FaqAssistant.Application.Features.User.Commands.CreateUser;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IHashService> _mockHashService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockHashService = new Mock<IHashService>();
        _handler = new CreateUserCommandHandler(_mockUnitOfWork.Object, _mockUserRepository.Object, _mockHashService.Object);
    }

    [Fact]
    public async Task Handle_ValidUserData_CreatesUserSuccessfully()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            UserName = "john_doe",
            Email = "john@example.com",
            Password = "SecurePassword123"
        };
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);
        _mockHashService.Setup(s => s.HashPassword(command.Password)).Returns("hashed_password");
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            UserName = "john_doe",
            Email = "invalid-email",
            Password = "SecurePassword123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email format.", result.Message);
    }

    [Fact]
    public async Task Handle_ShortPassword_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            UserName = "john_doe",
            Email = "john@example.com",
            Password = "short"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Password must be at least 6 characters long.", result.Message);
    }

    [Fact]
    public async Task Handle_EmptyUsername_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            UserName = "",
            Email = "john@example.com",
            Password = "SecurePassword123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username cannot be empty.", result.Message);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            UserName = "john_doe",
            Email = "existing@example.com",
            Password = "SecurePassword123"
        };
        var existingUser = new Domain.Entities.User { Email = "existing@example.com" };
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("A user with the same email or username already exists.", result.Message);
    }

    [Fact]
    public async Task Handle_HashServiceCalledWithCorrectPassword()
    {
        // Arrange
        var password = "TestPassword123";
        var command = new CreateUserCommand
        {
            UserName = "test_user",
            Email = "test@example.com",
            Password = password
        };
        _mockUserRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);
        _mockHashService.Setup(s => s.HashPassword(password)).Returns("hashed");
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockHashService.Verify(s => s.HashPassword(password), Times.Once);
    }
}
