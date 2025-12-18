using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.User.Commands.AuthorizeUser;
using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using MediatR;
using System.Linq.Expressions;
using UserDetails = FaqAssistant.Domain.Entities.User; 

namespace FaqAssistant.Application.Features.User.Commands.AuthorizeUserp;

public class AuthorizeUserCommandHandler : IRequestHandler<AuthorizeUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IHashService _hashService;

    public AuthorizeUserCommandHandler(IUserRepository userRepository, IJwtService jwtService, IHashService hashService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
    }
    public async Task<Result<AuthResponse>> Handle(AuthorizeUserCommand request, CancellationToken cancellationToken)
    {
        
        Expression<Func<UserDetails, bool>> predicate = (u => u.Username == request.Username);
        var user = await _userRepository.GetFirstAsync(predicate, cancellationToken);
        if (user == null || !_hashService.VerifyHashPassword(request.Password, user.PasswordHash))
        {
            return new Result<AuthResponse>()
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }
        var token = _jwtService.GenerateToken(user);
        return new Result<AuthResponse>()
        {
            Success = true,
            Data = new AuthResponse()
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
            },
        };
    }
}
