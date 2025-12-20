using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.QueryHandlers;

public class GetFaqDetailsQueryHandlerTests
{
    private readonly Mock<IFaqRepository> _mockFaqRepository;
    private readonly GetFaqDetailsQueryHandler _handler;

    public GetFaqDetailsQueryHandlerTests()
    {
        _mockFaqRepository = new Mock<IFaqRepository>();
        _handler = new GetFaqDetailsQueryHandler(_mockFaqRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsSuccessResult()
    {
        // Arrange
        var pageParams = new PageParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetFaqDetailsQuery(pageParams, null, null);
        var faqDetailsResponse = new GetFaqDetailsResponse(
            Guid.NewGuid(),
            "What is C#?",
            "C# is a modern programming language.",
            new FaqCategoryDto(Guid.NewGuid(), "Programming"),
            [new FaqTagsDto(Guid.NewGuid(), "CSharp")],
            DateTime.UtcNow,
            5,
            new FaqUserDto(Guid.NewGuid(), "john_doe")
        );
        var pagedResult = PagedResult<GetFaqDetailsResponse>.Create(
            [faqDetailsResponse],
            pageParams.PageNumber,
            pageParams.PageSize,
            1
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsAsync(pageParams, null, null))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data.Items);
        Assert.Equal("What is C#?", result.Data.Items.First().Question);
        _mockFaqRepository.Verify(r => r.GetFaqDetailsAsync(pageParams, null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ReturnsSuccessResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var pageParams = new PageParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetFaqDetailsQuery(pageParams, categoryId, null);
        var faqDetailsResponse = new GetFaqDetailsResponse(
            Guid.NewGuid(),
            "What is .NET?",
            ".NET is a framework.",
            new FaqCategoryDto(categoryId, "Framework"),
            [],
            DateTime.UtcNow,
            3,
            new FaqUserDto(Guid.NewGuid(), "jane_doe")
        );
        var pagedResult = PagedResult<GetFaqDetailsResponse>.Create(
            [faqDetailsResponse],
            pageParams.PageNumber,
            pageParams.PageSize,
            1
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsAsync(pageParams, categoryId, null))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Equal(categoryId, result.Data.Items.First().Category.Id);
    }

    [Fact]
    public async Task Handle_WithTagFilter_ReturnsSuccessResult()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var pageParams = new PageParameters { PageNumber = 2, PageSize = 5 };
        var query = new GetFaqDetailsQuery(pageParams, null, tagId);
        var faqDetailsResponse = new GetFaqDetailsResponse(
            Guid.NewGuid(),
            "How to use LINQ?",
            "LINQ is a query language.",
            new FaqCategoryDto(Guid.NewGuid(), "Advanced"),
            [new FaqTagsDto(tagId, "LINQ")],
            DateTime.UtcNow,
            8,
            new FaqUserDto(Guid.NewGuid(), "expert_user")
        );
        var pagedResult = PagedResult<GetFaqDetailsResponse>.Create(
            [faqDetailsResponse],
            pageParams.PageNumber,
            pageParams.PageSize,
            1
        );
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsAsync(pageParams, null, tagId))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);
        Assert.Contains(result.Data.Items.First().Tags, t => t.Id == tagId);
    }

    [Fact]
    public async Task Handle_WithNoResults_ReturnsEmptyPagedResult()
    {
        // Arrange
        var pageParams = new PageParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetFaqDetailsQuery(pageParams, null, null);
        var pagedResult = PagedResult<GetFaqDetailsResponse>.Create([], pageParams.PageNumber, pageParams.PageSize, 0);
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsAsync(pageParams, null, null))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }

    [Fact]
    public async Task Handle_WithMultipleResults_ReturnsAllItems()
    {
        // Arrange
        var pageParams = new PageParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetFaqDetailsQuery(pageParams, null, null);
        var faqList = Enumerable.Range(1, 3)
            .Select(i => new GetFaqDetailsResponse(
                Guid.NewGuid(),
                $"Question {i}",
                $"Answer {i}",
                new FaqCategoryDto(Guid.NewGuid(), $"Category {i}"),
                [],
                DateTime.UtcNow,
                i,
                new FaqUserDto(Guid.NewGuid(), $"user_{i}")
            ))
            .ToList();
        var pagedResult = PagedResult<GetFaqDetailsResponse>.Create(faqList, pageParams.PageNumber, pageParams.PageSize, 3);
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsAsync(pageParams, null, null))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Data.Items.Count);
        Assert.True(result.Data.HasNextPage == false);
        Assert.True(result.Data.HasPreviousPage == false);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsPaginationMetadata()
    {
        // Arrange
        var pageParams = new PageParameters { PageNumber = 2, PageSize = 5 };
        var query = new GetFaqDetailsQuery(pageParams, null, null);
        var faqList = Enumerable.Range(1, 5).Select(i => new GetFaqDetailsResponse(
            Guid.NewGuid(), $"Q{i}", $"A{i}", new FaqCategoryDto(Guid.NewGuid(), "Cat"), [], DateTime.UtcNow, 0, new FaqUserDto(Guid.NewGuid(), "user")
        )).ToList();
        var pagedResult = PagedResult<GetFaqDetailsResponse>.Create(faqList, 2, 5, 15);
        _mockFaqRepository
            .Setup(r => r.GetFaqDetailsAsync(pageParams, null, null))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Data.HasNextPage);
        Assert.True(result.Data.HasPreviousPage);
        Assert.Equal(2, result.Data.PageNumber);
        Assert.Equal(5, result.Data.PageSize);
        Assert.Equal(15, result.Data.TotalCount);
        Assert.Equal(3, result.Data.TotalPages);
    }
}
