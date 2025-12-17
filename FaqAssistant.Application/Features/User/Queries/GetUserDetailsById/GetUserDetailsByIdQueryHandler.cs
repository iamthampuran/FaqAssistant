using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;

public class GetUserDetailsByIdQueryHandler : IRequestHandler<GetUserDetailsByIdQuery, Result<GetUserByIdResponse>>
{
    private readonly IUserRepository _userRepository;
    public GetUserDetailsByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<GetUserByIdResponse>> Handle(GetUserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return new Result<GetUserByIdResponse>(false, "User not found.");
        }

        return new Result<GetUserByIdResponse>(true, user);
    }
}
