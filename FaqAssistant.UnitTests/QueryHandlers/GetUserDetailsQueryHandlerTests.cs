using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.User.Queries.GetUserDetails;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetUserDetailsQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUserDetailsQueryHandler _handler;

    public GetUserDetailsQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUserDetailsQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetUserDetailsQuery(10, 1, null);
        var userDetails = new GetUserDetailsResponse(Guid.NewGuid(), "john_doe", "john@example.com", DateTime.UtcNow);
        var pagedResult = PagedResult<GetUserDetailsResponse>.Create([userDetails], 1, 10, 1);
        _mockUserRepository
            .Setup(r => r.GetUserDetailsAsync(query.PageCount, query.PageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Equal("john_doe", result.Data.Items.First().Username);
    }

    [Fact]
    public async Task Handle_WithSearchValue_ReturnsFilteredResults()
    {
        // Arrange
        var query = new GetUserDetailsQuery(10, 1, "admin");
        var userDetails = new GetUserDetailsResponse(Guid.NewGuid(), "admin_user", "admin@example.com", DateTime.UtcNow);
        var pagedResult = PagedResult<GetUserDetailsResponse>.Create([userDetails], 1, 10, 1);
        _mockUserRepository
            .Setup(r => r.GetUserDetailsAsync(query.PageCount, query.PageSize, "admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Contains("admin", result.Data.Items.First().Username);
    }

    [Fact]
    public async Task Handle_WithNoResults_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetUserDetailsQuery(10, 1, "nonexistent");
        var pagedResult = PagedResult<GetUserDetailsResponse>.Create([], 1, 10, 0);
        _mockUserRepository
            .Setup(r => r.GetUserDetailsAsync(query.PageCount, query.PageSize, "nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }

    [Fact]
    public async Task Handle_WithMultipleResults_ReturnsPaginatedData()
    {
        // Arrange
        var query = new GetUserDetailsQuery(5, 2, null);
        var users = Enumerable.Range(1, 5)
            .Select(i => new GetUserDetailsResponse(Guid.NewGuid(), $"user_{i}", $"user{i}@example.com", DateTime.UtcNow))
            .ToList();
        var pagedResult = PagedResult<GetUserDetailsResponse>.Create(users, 2, 5, 12);
        _mockUserRepository
            .Setup(r => r.GetUserDetailsAsync(query.PageCount, query.PageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Data.Items.Count);
        Assert.Equal(12, result.Data.TotalCount);
        Assert.True(result.Data.HasNextPage);
        Assert.True(result.Data.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_WithPagination_VerifiesRepositoryCall()
    {
        // Arrange
        var pageSize = 20;
        var pageCount = 3;
        var query = new GetUserDetailsQuery(pageSize, pageCount, null);
        var pagedResult = PagedResult<GetUserDetailsResponse>.Create([], pageCount, pageSize, 0);
        _mockUserRepository
            .Setup(r => r.GetUserDetailsAsync(pageCount, pageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(r => r.GetUserDetailsAsync(pageCount, pageSize, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
