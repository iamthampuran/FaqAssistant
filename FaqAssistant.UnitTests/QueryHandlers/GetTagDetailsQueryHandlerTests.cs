using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetTagDetailsQueryHandlerTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly GetTagDetailsQueryHandler _handler;

    public GetTagDetailsQueryHandlerTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _handler = new GetTagDetailsQueryHandler(_mockTagRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetTagDetailsQuery(10, 1, null);
        var tagDetails = new GetTagDetailsResponse(Guid.NewGuid(), "csharp", DateTime.UtcNow);
        var pagedResult = PagedResult<GetTagDetailsResponse>.Create([tagDetails], 1, 10, 1);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsAsync(query.PageCount, query.PageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Equal("csharp", result.Data.Items.First().Name);
    }

    [Fact]
    public async Task Handle_WithSearchValue_ReturnsFilteredTags()
    {
        // Arrange
        var query = new GetTagDetailsQuery(10, 1, "aspnet");
        var tagDetails = new GetTagDetailsResponse(Guid.NewGuid(), "aspnet-core", DateTime.UtcNow);
        var pagedResult = PagedResult<GetTagDetailsResponse>.Create([tagDetails], 1, 10, 1);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsAsync(query.PageCount, query.PageSize, "aspnet", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Contains("aspnet", result.Data.Items.First().Name);
    }

    [Fact]
    public async Task Handle_WithNoResults_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetTagDetailsQuery(10, 1, "nonexistent");
        var pagedResult = PagedResult<GetTagDetailsResponse>.Create([], 1, 10, 0);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsAsync(query.PageCount, query.PageSize, "nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
    }

    [Fact]
    public async Task Handle_WithMultipleTags_ReturnsPaginatedData()
    {
        // Arrange
        var query = new GetTagDetailsQuery(5, 1, null);
        var tags = Enumerable.Range(1, 5)
            .Select(i => new GetTagDetailsResponse(Guid.NewGuid(), $"tag_{i}", DateTime.UtcNow))
            .ToList();
        var pagedResult = PagedResult<GetTagDetailsResponse>.Create(tags, 1, 5, 25);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsAsync(query.PageCount, query.PageSize, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Data.Items.Count);
        Assert.Equal(25, result.Data.TotalCount);
        Assert.Equal(5, result.Data.TotalPages);
    }

    [Fact]
    public async Task Handle_VerifiesRepositoryCall()
    {
        // Arrange
        var query = new GetTagDetailsQuery(15, 2, "test");
        var pagedResult = PagedResult<GetTagDetailsResponse>.Create([], 2, 15, 0);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsAsync(query.PageCount, query.PageSize, "test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockTagRepository.Verify(r => r.GetTagDetailsAsync(query.PageCount, query.PageSize, "test", It.IsAny<CancellationToken>()), Times.Once);
    }
}
