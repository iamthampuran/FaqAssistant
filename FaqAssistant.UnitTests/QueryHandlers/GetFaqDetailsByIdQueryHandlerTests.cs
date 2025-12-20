using FaqAssistant.Application.Features.Faq.Queries.GetFaqDetailsById;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetFaqDetailsByIdQueryHandlerTests
{
    private readonly Mock<IFaqRepository> _mockFaqRepository;
    private readonly GetFaqDetailsByIdQueryHandler _handler;

    public GetFaqDetailsByIdQueryHandlerTests()
    {
        _mockFaqRepository = new Mock<IFaqRepository>();
        _handler = new GetFaqDetailsByIdQueryHandler(_mockFaqRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidFaqId_ReturnsSuccessResult()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var query = new GetFaqDetailsByIdQuery(faqId);
        var faqDetailsResponse = new GetFaqDetailsByIdResponse(
            faqId,
            "What is ASP.NET Core?",
            "ASP.NET Core is a web framework.",
            new FaqCategoryDto(Guid.NewGuid(), "Web Development"),
            [new FaqTagsDto(Guid.NewGuid(), "ASP.NET")],
            DateTime.UtcNow,
            10,
            new FaqUserDto(Guid.NewGuid(), "web_expert")
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsByIdAsync(faqId))
            .ReturnsAsync(faqDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(faqId, result.Data.Id);
        Assert.Equal("What is ASP.NET Core?", result.Data.Question);
        Assert.Null(result.Message);
        _mockFaqRepository.Verify(r => r.GetFaqDetailsByIdAsync(faqId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentFaqId_ReturnsFailureResult()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var query = new GetFaqDetailsByIdQuery(faqId);
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsByIdAsync(faqId))
            .ReturnsAsync((GetFaqDetailsByIdResponse?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("FAQ not found.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsCompleteDetailData()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow.AddDays(-5);
        var query = new GetFaqDetailsByIdQuery(faqId);
        var faqDetailsResponse = new GetFaqDetailsByIdResponse(
            faqId,
            "How to implement dependency injection?",
            "DI is a pattern used to create loose coupling.",
            new FaqCategoryDto(categoryId, "Design Patterns"),
            [
                new FaqTagsDto(Guid.NewGuid(), "DependencyInjection"),
                new FaqTagsDto(Guid.NewGuid(), "DesignPatterns"),
                new FaqTagsDto(Guid.NewGuid(), "CSharp")
            ],
            createdDate,
            15,
            new FaqUserDto(userId, "architect")
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsByIdAsync(faqId))
            .ReturnsAsync(faqDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        var data = result.Data;
        Assert.Equal(faqId, data.Id);
        Assert.Equal("How to implement dependency injection?", data.Question);
        Assert.Equal("DI is a pattern used to create loose coupling.", data.Answer);
        Assert.Equal(categoryId, data.Category.Id);
        Assert.Equal("Design Patterns", data.Category.Name);
        Assert.Equal(3, data.Tags.Count);
        Assert.Contains(data.Tags, t => t.Name == "DependencyInjection");
        Assert.Equal(15, data.Rating);
        Assert.Equal(userId, data.UserDetails.Id);
        Assert.Equal("architect", data.UserDetails.Username);
        Assert.Equal(createdDate, data.CreatedAt);
    }

    [Fact]
    public async Task Handle_WithMultipleTags_ReturnsAllTags()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var query = new GetFaqDetailsByIdQuery(faqId);
        var tags = new List<FaqTagsDto>
        {
            new(Guid.NewGuid(), "Tag1"),
            new(Guid.NewGuid(), "Tag2"),
            new(Guid.NewGuid(), "Tag3"),
            new(Guid.NewGuid(), "Tag4")
        };
        var faqDetailsResponse = new GetFaqDetailsByIdResponse(
            faqId,
            "Question",
            "Answer",
            new FaqCategoryDto(Guid.NewGuid(), "Category"),
            tags,
            DateTime.UtcNow,
            5,
            new FaqUserDto(Guid.NewGuid(), "user")
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsByIdAsync(faqId))
            .ReturnsAsync(faqDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(4, result.Data.Tags.Count);
        Assert.Equal(tags, result.Data.Tags);
    }

    [Fact]
    public async Task Handle_WithZeroRating_ReturnsCorrectRating()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var query = new GetFaqDetailsByIdQuery(faqId);
        var faqDetailsResponse = new GetFaqDetailsByIdResponse(
            faqId,
            "New Question",
            "New Answer",
            new FaqCategoryDto(Guid.NewGuid(), "New"),
            [],
            DateTime.UtcNow,
            0,
            new FaqUserDto(Guid.NewGuid(), "newuser")
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsByIdAsync(faqId))
            .ReturnsAsync(faqDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Data.Rating);
    }

    [Fact]
    public async Task Handle_WithHighRating_ReturnsCorrectRating()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var query = new GetFaqDetailsByIdQuery(faqId);
        var faqDetailsResponse = new GetFaqDetailsByIdResponse(
            faqId,
            "Popular Question",
            "Popular Answer",
            new FaqCategoryDto(Guid.NewGuid(), "Popular"),
            [],
            DateTime.UtcNow,
            100,
            new FaqUserDto(Guid.NewGuid(), "popular_user")
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsByIdAsync(faqId))
            .ReturnsAsync(faqDetailsResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100, result.Data.Rating);
    }
}
