using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using MediatR;
using System.Linq.Expressions;
using UserDetails = FaqAssistant.Domain.Entities.User;

namespace FaqAssistant.Application.Features.User.Commands.CreateUser;
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<UserDetails> _userRepository;
    private readonly IHashService _hashService;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IGenericRepository<UserDetails> userRepository, IHashService hashService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
    }
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Simulate user creation logic
        var newUserId = Guid.NewGuid();
        
        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
        {
            return new Result<Guid>(false, "Password must be at least 6 characters long.");
        }

        if (string.IsNullOrEmpty(request.Email) || !SpecificValidators.IsValidEmail(request.Email))
        {
           return new Result<Guid>(false, "Invalid email format.");
        }

        if (string.IsNullOrEmpty(request.UserName))
        {
            return new Result<Guid>(false, "Username cannot be empty.");
        }

        Expression<Func<UserDetails, bool>> predicate = (u => u.Email == request.Email || u.Username == request.UserName);
        var existingUser = await _userRepository.GetFirstAsync(predicate, cancellationToken);

        if (existingUser != null)
        {
            return new Result<Guid>(false, "A user with the same email or username already exists.");
        }
        
        var newUser = new UserDetails
        {
            Id = newUserId,
            Username = request.UserName,
            Email = request.Email,
            PasswordHash = _hashService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        // In a real application, you would add code here to save the user to a database
        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new Result<Guid>(true, newUserId, "User created successfully.");
    }
}