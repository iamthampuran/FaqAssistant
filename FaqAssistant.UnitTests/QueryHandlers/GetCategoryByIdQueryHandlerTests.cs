using FaqAssistant.Application.Features.Category.Queries.GetCategoryById;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new GetCategoryByIdQueryHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCategoryId_ReturnsSuccessResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryByIdQuery(categoryId);
        var createdDate = DateTime.UtcNow.AddDays(-5);
        var categoryEntity = new Domain.Entities.Category 
        { 
            Id = categoryId, 
            Name = "Programming", 
            CreatedAt = createdDate,
            IsDeleted = false
        };
        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryEntity);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(categoryId, result.Data.Id);
        Assert.Equal("Programming", result.Data.Name);
        Assert.Equal(createdDate, result.Data.CreatedAt);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategoryId_ReturnsFailureResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryByIdQuery(categoryId);
        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Category?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Category not found.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectCategoryData()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = "Web Development";
        var createdDate = DateTime.UtcNow;
        var query = new GetCategoryByIdQuery(categoryId);
        var categoryEntity = new Domain.Entities.Category
        {
            Id = categoryId,
            Name = categoryName,
            CreatedAt = createdDate,
            IsDeleted = false
        };
        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryEntity);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(categoryId, result.Data.Id);
        Assert.Equal(categoryName, result.Data.Name);
        Assert.Equal(createdDate, result.Data.CreatedAt);
    }

    [Fact]
    public async Task Handle_VerifiesRepositoryCall()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetCategoryByIdQuery(categoryId);
        var categoryEntity = new Domain.Entities.Category
        {
            Id = categoryId,
            Name = "Test",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryEntity);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentCategoryIds_ReturnCorrectData()
    {
        // Arrange
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        var query1 = new GetCategoryByIdQuery(categoryId1);
        var query2 = new GetCategoryByIdQuery(categoryId2);
        var category1 = new Domain.Entities.Category
        {
            Id = categoryId1,
            Name = "Category1",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        var category2 = new Domain.Entities.Category
        {
            Id = categoryId2,
            Name = "Category2",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category1);
        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category2);

        // Act
        var result1 = await _handler.Handle(query1, CancellationToken.None);
        var result2 = await _handler.Handle(query2, CancellationToken.None);

        // Assert
        Assert.Equal("Category1", result1.Data.Name);
        Assert.Equal("Category2", result2.Data.Name);
    }
}
