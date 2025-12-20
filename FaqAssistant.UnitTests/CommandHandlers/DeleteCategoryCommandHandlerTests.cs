using FaqAssistant.Application.Features.Category.Commands.DeleteCategory;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteCategoryCommandHandler(_mockCategoryRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidDeletion_DeletesCategorySuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };
        var category = new Domain.Entities.Category { Id = categoryId, IsDeleted = false };
        
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(categoryId, result.Data);
        Assert.Equal("Category deleted successfully.", result.Message);
        Assert.True(category.IsDeleted);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Category?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Category not found.", result.Message);
    }

    [Fact]
    public async Task Handle_AlreadyDeletedCategory_ReturnsFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };
        var deletedCategory = new Domain.Entities.Category { Id = categoryId, IsDeleted = true };
        
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(deletedCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Category not found.", result.Message);
    }

    [Fact]
    public async Task Handle_SetsIsDeletedFlagAndUpdatesTimestamp()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };
        var category = new Domain.Entities.Category 
        { 
            Id = categoryId, 
            IsDeleted = false,
            LastUpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(category.IsDeleted);
        Assert.True(category.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }
}
