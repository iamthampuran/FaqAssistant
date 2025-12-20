using FaqAssistant.Application.Features.Faq.Commands.CreateFaq;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace FaqAssistant.UnitTests.CommandHandlers
{
    public class AddFaqCommandHandlerTests
    {
        private readonly Mock<IFaqRepository> _faqRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<ITagRepository> _tagRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateFaqCommandHandler _handler;

        public AddFaqCommandHandlerTests()
        {
            _faqRepositoryMock = new Mock<IFaqRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _tagRepositoryMock = new Mock<ITagRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new CreateFaqCommandHandler(
                _faqRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateFaqCommand
            {
                UserId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid>()
            };
            _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("User is not authenticated.", result.Message);
        }

        [Fact]
        public async Task Handle_UserIdMismatch_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateFaqCommand
            {
                UserId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid>()
            };
            _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("You are not authorized to add details for other user.", result.Message);
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateFaqCommand
            {
                UserId = userId,
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid>()
            };
            _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Category not found.", result.Message);
        }

        [Fact]
        public async Task Handle_TagDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateFaqCommand
            {
                UserId = userId,
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid() }
            };
            _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>?>()!))
                .ReturnsAsync([]);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("One or more tags not found.", result.Message);
        }

        [Fact]
        public async Task Handle_ValidRequest_AddsFaqSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateFaqCommand
            {
                UserId = userId,
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid() }
            };
            _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
            _categoryRepositoryMock.Setup(r => r.ExistsAsync(command.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>?>()!))
                .ReturnsAsync(new List<Tag> { new Tag { Id = command.TagIds[0] } });
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.Success);
            _faqRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Faq>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
