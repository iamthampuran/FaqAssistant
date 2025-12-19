using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;

public class GetUserDetailsByIdQueryHandler : IRequestHandler<GetUserDetailsByIdQuery, Result<GetUserDetailsByIdQueryResponse>>
{
    private readonly IUserRepository _userRepository;
    public GetUserDetailsByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<GetUserDetailsByIdQueryResponse>> Handle(GetUserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return new Result<GetUserDetailsByIdQueryResponse>(false, "User not found.");
        }

        return new Result<GetUserDetailsByIdQueryResponse>(true, user);
    }
}
