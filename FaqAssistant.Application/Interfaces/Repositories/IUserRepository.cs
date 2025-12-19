using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.User.Queries.GetUserDetails;
using FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;
using FaqAssistant.Domain.Entities;

namespace FaqAssistant.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<GetUserDetailsByIdQueryResponse?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task<PagedResult<GetUserDetailsQueryResponse>> GetUserDetailsAsync(int pageNumber, int pageSize, string? searchValue, CancellationToken cancellationToken = default);

}
