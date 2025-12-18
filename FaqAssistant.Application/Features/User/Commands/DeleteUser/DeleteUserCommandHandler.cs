using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using MediatR;

namespace FaqAssistant.Application.Features.User.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandHandler(
        IUnitOfWork unitOfWork, 
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }
    
    public async Task<Result<Guid>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return new Result<Guid>(false, "User is not authenticated.");
        }

        if (currentUserId != request.UserId)
        {
            return new Result<Guid>(false, "You are not authorized to delete this user's account.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return new Result<Guid>(false, "User not found.");
        }
        // Simulate deletion logic
        user.IsDeleted = true;
        user.LastUpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new Result<Guid>(true, request.UserId);
    }
}
