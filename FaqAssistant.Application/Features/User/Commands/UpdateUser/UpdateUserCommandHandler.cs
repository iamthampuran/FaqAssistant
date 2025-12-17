using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;
using System.Linq.Expressions;
using UserDetails = FaqAssistant.Domain.Entities.User;


namespace FaqAssistant.Application.Features.User.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return new Result<Guid>(false, "User not found.");
        }

        if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.UserName))
        {
            return new Result<Guid>(false, "At least one field (Email or UserName) must be provided for update.");
        }

        Expression<Func<UserDetails, bool>> predicate = (u => u.Email == request.Email || u.Username == request.UserName);
        var existingUser = await _userRepository.GetFirstAsync(predicate, cancellationToken);

        if (existingUser != null && existingUser.Id != request.Id)
        {
            return new Result<Guid>(false, "A user with the same email or username already exists.");
        }

        user.Email = request.Email ?? user.Email;
        user.Username = request.UserName ?? user.Username;
        user.LastUpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Simulate update logic
        // In a real implementation, you would update the user entity and save changes to the database
        return new Result<Guid>(true, request.Id);
    }
}
