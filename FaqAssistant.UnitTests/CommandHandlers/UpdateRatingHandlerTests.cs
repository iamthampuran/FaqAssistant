using FaqAssistant.Application.Features.Faq.Commands.UpdateRating;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers;

public class UpdateRatingHandlerTests
{
    private readonly Mock<IFaqRepository> _mockFaqRepository;
    private readonly Mock<IGenericRepository<Rating>> _mockRatingRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly UpdateRatingCommandHandler _handler;

    public UpdateRatingHandlerTests()
    {
        _mockFaqRepository = new Mock<IFaqRepository>();
        _mockRatingRepository = new Mock<IGenericRepository<Rating>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();

        _handler = new UpdateRatingCommandHandler(
            _mockFaqRepository.Object,
            _mockRatingRepository.Object,
            _mockUnitOfWork.Object,
            _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateRatingCommand(Guid.NewGuid(), true);

        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User is not authenticated.", result.Message);
    }

    [Fact]
    public async Task Handle_FaqNotFound_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateRatingCommand(Guid.NewGuid(), true);
        var userId = Guid.NewGuid();
        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(userId);
        _mockFaqRepository
            .Setup(r => r.GetFirstAsync(
                It.IsAny<Expression<Func<Faq, bool>>>(),
                It.IsAny<List<Expression<Func<Faq, object>>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Faq?)null);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.Success);
        Assert.Equal("Faq not found.", result.Message);
    }

    [Fact]
    public async Task Handle_ExistingRatingSameAsRequest_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateRatingCommand(Guid.NewGuid(), true);
        var userId = Guid.NewGuid();
        var faq = new Faq
        {
            Id = command.Id,
            Ratings = new List<Rating>
            {
                new Rating
                {
                    UserId = userId,
                    IsUpvote = true,
                    IsDeleted = false
                }
            }
        };
        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(userId);
        _mockFaqRepository
            .Setup(r => r.GetFirstAsync(
                It.IsAny<Expression<Func<Faq, bool>>>(),
                It.IsAny<List<Expression<Func<Faq, object>>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.Success);
        Assert.Equal("You have already submitted this rating.", result.Message);
    }

    [Fact]
    public async Task Handle_UpdateExistingRating_Succeeds()
    {
        // Arrange
        var command = new UpdateRatingCommand(Guid.NewGuid(), false);
        var userId = Guid.NewGuid();
        var faq = new Faq
        {
            Id = command.Id,
            Ratings = new List<Rating>
            {
                new Rating
                {
                    UserId = userId,
                    IsUpvote = true,
                    IsDeleted = false
                }
            }
        };
        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(userId);
        _mockFaqRepository
            .Setup(r => r.GetFirstAsync(
                It.IsAny<Expression<Func<Faq, bool>>>(),
                It.IsAny<List<Expression<Func<Faq, object>>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.Success);
        Assert.Equal(faq.Id, result.Data);
        var updatedRating = faq.Ratings.First(r => r.UserId == userId);
        Assert.False(updatedRating.IsUpvote);
    }

    [Fact]
    public async Task Handle_AddNewRating_Succeeds()
    {
        // Arrange
        var command = new UpdateRatingCommand(Guid.NewGuid(), true);
        var userId = Guid.NewGuid();
        var faq = new Faq
        {
            Id = command.Id,
            Ratings = new List<Rating>()
        };
        _mockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(userId);
        _mockFaqRepository
            .Setup(r => r.GetFirstAsync(
                It.IsAny<Expression<Func<Faq, bool>>>(),
                It.IsAny<List<Expression<Func<Faq, object>>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(faq);
        _mockRatingRepository
            .Setup(r => r.AddAsync(It.IsAny<Rating>(), It.IsAny<CancellationToken>()))
            .Callback<Rating, CancellationToken>((rating, ct) => faq.Ratings.Add(rating))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.Success);
        Assert.Equal(faq.Id, result.Data);
        var newRating = faq.Ratings.FirstOrDefault(r => r.UserId == userId);
        Assert.NotNull(newRating);
        Assert.True(newRating.IsUpvote);
    }
}
