using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using MediatR;
using System.Linq.Expressions;
using UserDetails = FaqAssistant.Domain.Entities.User;


namespace FaqAssistant.Application.Features.User.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    
    public UpdateUserCommandHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return new Result<Guid>(false, "User is not authenticated.");
        }

        if (currentUserId != request.Id)
        {
            return new Result<Guid>(false, "You are not authorized to update this user's details.");
        }

        // Combine the queries to fetch the user and check for conflicts in one call
        var userWithConflicts = await _userRepository.GetFirstAsync(
            u => (u.Id == request.Id || u.Email == request.Email || u.Username == request.UserName) && !u.IsDeleted,
            cancellationToken
        );

        if (userWithConflicts == null)
        {
            return new Result<Guid>(false, "User not found.");
        }

        if (userWithConflicts.Id != request.Id)
        {
            return new Result<Guid>(false, "A user with the same email or username already exists.");
        }

        // Update the user details
        userWithConflicts.Email = request.Email ?? userWithConflicts.Email;
        userWithConflicts.Username = request.UserName ?? userWithConflicts.Username;
        userWithConflicts.LastUpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Result<Guid>(true, request.Id);
    }
}
