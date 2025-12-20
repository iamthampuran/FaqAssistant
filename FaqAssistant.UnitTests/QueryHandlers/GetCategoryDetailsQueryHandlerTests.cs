using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Category.Queries.GetCategoryDetails;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetCategoryDetailsQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly GetCategoryDetailsQueryHandler _handler;

    public GetCategoryDetailsQueryHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new GetCategoryDetailsQueryHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetCategoryDetailsQuery(10, 1, null);
        var categoryDetails = new GetCategoryDetailsResponse(Guid.NewGuid(), "Programming", DateTime.UtcNow);
        var pagedResult = PagedResult<GetCategoryDetailsResponse>.Create([categoryDetails], 1, 10, 1);
        _mockCategoryRepository
            .Setup(r => r.GetCategoryDetailsAsync(query.PageCount, query.PageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Equal("Programming", result.Data.Items.First().Name);
    }

    [Fact]
    public async Task Handle_WithSearchValue_ReturnsFilteredCategories()
    {
        // Arrange
        var query = new GetCategoryDetailsQuery(10, 1, "web");
        var categoryDetails = new GetCategoryDetailsResponse(Guid.NewGuid(), "Web Development", DateTime.UtcNow);
        var pagedResult = PagedResult<GetCategoryDetailsResponse>.Create([categoryDetails], 1, 10, 1);
        _mockCategoryRepository
            .Setup(r => r.GetCategoryDetailsAsync(query.PageCount, query.PageSize, "web", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Contains("Web", result.Data.Items.First().Name);
    }

    [Fact]
    public async Task Handle_WithNoResults_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCategoryDetailsQuery(10, 1, "nonexistent");
        var pagedResult = PagedResult<GetCategoryDetailsResponse>.Create([], 1, 10, 0);
        _mockCategoryRepository
            .Setup(r => r.GetCategoryDetailsAsync(query.PageCount, query.PageSize, "nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }

    [Fact]
    public async Task Handle_WithMultipleCategories_ReturnsPaginatedData()
    {
        // Arrange
        var query = new GetCategoryDetailsQuery(5, 1, null);
        var categories = Enumerable.Range(1, 5)
            .Select(i => new GetCategoryDetailsResponse(Guid.NewGuid(), $"Category_{i}", DateTime.UtcNow))
            .ToList();
        var pagedResult = PagedResult<GetCategoryDetailsResponse>.Create(categories, 1, 5, 20);
        _mockCategoryRepository
            .Setup(r => r.GetCategoryDetailsAsync(query.PageCount, query.PageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Data.Items.Count);
        Assert.Equal(20, result.Data.TotalCount);
        Assert.Equal(4, result.Data.TotalPages);
    }

    [Fact]
    public async Task Handle_VerifiesRepositoryCall()
    {
        // Arrange
        var query = new GetCategoryDetailsQuery(15, 2, "test");
        var pagedResult = PagedResult<GetCategoryDetailsResponse>.Create([], 2, 15, 0);
        _mockCategoryRepository
            .Setup(r => r.GetCategoryDetailsAsync(query.PageCount, query.PageSize, "test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockCategoryRepository.Verify(r => r.GetCategoryDetailsAsync(query.PageCount, query.PageSize, "test", It.IsAny<CancellationToken>()), Times.Once);
    }
}
