using FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetUserDetailsByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUserDetailsByIdQueryHandler _handler;

    public GetUserDetailsByIdQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUserDetailsByIdQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ReturnsSuccessResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserDetailsByIdQuery(userId);
        var userDetailsResponse = new GetUserDetailsByIdResponse(userId, "john_doe", "john@example.com");
        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal("john_doe", result.Data.UserName);
        Assert.Equal("john@example.com", result.Data.Email);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task Handle_WithNonExistentUserId_ReturnsFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserDetailsByIdQuery(userId);
        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetUserDetailsByIdResponse?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task Handle_VerifiesRepositoryCallWithCorrectUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserDetailsByIdQuery(userId);
        var userDetailsResponse = new GetUserDetailsByIdResponse(userId, "test_user", "test@example.com");
        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDetailsResponse);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsDifferentUserIds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserDetailsByIdQuery(userId);
        var userDetailsResponse = new GetUserDetailsByIdResponse(userId, "unique_user", "unique@example.com");
        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(userId, result.Data.UserId);
    }
}
