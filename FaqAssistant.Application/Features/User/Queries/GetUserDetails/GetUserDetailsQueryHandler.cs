using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.User.Queries.GetUserDetails;

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, Result<PagedResult<GetUserDetailsQueryResponse>>>
{
    private readonly IUserRepository _userRepository;
    public GetUserDetailsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }
    public async Task<Result<PagedResult<GetUserDetailsQueryResponse>>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {

        var pagedUsers = await _userRepository.GetUserDetailsAsync(request.PageCount, request.PageSize, request.SearchValue, cancellationToken);
        return new Result<PagedResult<GetUserDetailsQueryResponse>>(true, pagedUsers);
    }
}
