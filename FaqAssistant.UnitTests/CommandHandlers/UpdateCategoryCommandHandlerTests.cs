using FaqAssistant.Application.Features.Category.Commands.UpdateCategory;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesCategorySuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "Updated Category" };
        var category = new Domain.Entities.Category { Id = categoryId, Name = "Old Category" };
        
        _mockCategoryRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(categoryId, result.Data);
        Assert.Equal("Updated Category", category.Name);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "New Name" };
        _mockCategoryRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Category?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Category not found.", result.Message);
    }

    [Fact]
    public async Task Handle_DuplicateCategoryName_ReturnsFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "Duplicate Name" };
        var otherCategory = new Domain.Entities.Category { Id = otherCategoryId, Name = "Duplicate Name" };
        
        _mockCategoryRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Category with the same name already exists.", result.Message);
    }

    [Fact]
    public async Task Handle_DeletedCategory_ReturnsFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "New Name" };
        var deletedCategory = new Domain.Entities.Category { Id = categoryId, IsDeleted = true };
        
        _mockCategoryRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Category not found.", result.Message);
    }

    [Fact]
    public async Task Handle_UpdatesLastUpdatedAtTimestamp()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "Updated" };
        var category = new Domain.Entities.Category { Id = categoryId, Name = "Old", LastUpdatedAt = DateTime.UtcNow.AddHours(-1) };
        
        _mockCategoryRepository
            .Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Domain.Entities.Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(category.LastUpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }
}
