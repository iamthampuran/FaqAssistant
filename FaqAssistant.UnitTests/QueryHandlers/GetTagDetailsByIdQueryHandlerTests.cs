using FaqAssistant.Application.Features.Tag.Queries.GetTagDetailsById;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetTagDetailsByIdQueryHandlerTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly GetTagDetailsByIdQueryHandler _handler;

    public GetTagDetailsByIdQueryHandlerTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _handler = new GetTagDetailsByIdQueryHandler(_mockTagRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidTagId_ReturnsSuccessResult()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var query = new GetTagDetailsByIdQuery(tagId);
        var tagDetailsResponse = new GetTagDetailsByIdResponse(tagId, "csharp", DateTime.UtcNow);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsById(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(tagId, result.Data.Id);
        Assert.Equal("csharp", result.Data.Name);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task Handle_WithNonExistentTagId_ReturnsFailureResult()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var query = new GetTagDetailsByIdQuery(tagId);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsById(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetTagDetailsByIdResponse?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Tag not found.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsCompleteTagDetails()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow.AddDays(-10);
        var query = new GetTagDetailsByIdQuery(tagId);
        var tagDetailsResponse = new GetTagDetailsByIdResponse(tagId, "aspnet-core", createdDate);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsById(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(tagId, result.Data.Id);
        Assert.Equal("aspnet-core", result.Data.Name);
        Assert.Equal(createdDate, result.Data.CreatedAt);
    }

    [Fact]
    public async Task Handle_VerifiesRepositoryCall()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var query = new GetTagDetailsByIdQuery(tagId);
        var tagDetailsResponse = new GetTagDetailsByIdResponse(tagId, "test", DateTime.UtcNow);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsById(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagDetailsResponse);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockTagRepository.Verify(r => r.GetTagDetailsById(tagId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentTagIds_ReturnCorrectData()
    {
        // Arrange
        var tagId1 = Guid.NewGuid();
        var tagId2 = Guid.NewGuid();
        var query1 = new GetTagDetailsByIdQuery(tagId1);
        var query2 = new GetTagDetailsByIdQuery(tagId2);
        var response1 = new GetTagDetailsByIdResponse(tagId1, "tag1", DateTime.UtcNow);
        var response2 = new GetTagDetailsByIdResponse(tagId2, "tag2", DateTime.UtcNow);
        
        _mockTagRepository
            .Setup(r => r.GetTagDetailsById(tagId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1);
        _mockTagRepository
            .Setup(r => r.GetTagDetailsById(tagId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response2);

        // Act
        var result1 = await _handler.Handle(query1, CancellationToken.None);
        var result2 = await _handler.Handle(query2, CancellationToken.None);

        // Assert
        Assert.Equal("tag1", result1.Data.Name);
        Assert.Equal("tag2", result2.Data.Name);
    }
}
