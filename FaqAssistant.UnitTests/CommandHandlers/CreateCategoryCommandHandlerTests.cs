using FaqAssistant.Application.Features.Category.Commands.CreateCategory;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _handler = new CreateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCategoryName_CreatesCategorySuccessfully()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Programming" };
        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Category created successfully.", result.Message);
        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreatesCategory_WithCorrectName()
    {
        // Arrange
        var categoryName = "Web Development";
        var command = new CreateCategoryCommand { Name = categoryName };
        Domain.Entities.Category? capturedCategory = null;
        
        _mockCategoryRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Category, CancellationToken>((cat, ct) => capturedCategory = cat)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(capturedCategory);
        Assert.Equal(categoryName, capturedCategory.Name);
    }

    [Fact]
    public async Task Handle_GeneratesUniqueId()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Design" };
        Domain.Entities.Category? capturedCategory = null;
        
        _mockCategoryRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Category, CancellationToken>((cat, ct) => capturedCategory = cat)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Data);
        Assert.NotEqual(Guid.Empty, capturedCategory!.Id);
    }

    [Fact]
    public async Task Handle_MultipleCategories_GenerateDifferentIds()
    {
        // Arrange
        var command1 = new CreateCategoryCommand { Name = "Category1" };
        var command2 = new CreateCategoryCommand { Name = "Category2" };
        var capturedIds = new List<Guid>();
        
        _mockCategoryRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Category>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Category, CancellationToken>((cat, ct) => capturedIds.Add(cat.Id))
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.Data, result2.Data);
    }
}
