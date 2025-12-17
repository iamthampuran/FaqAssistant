using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.User.Queries.GetUserDetails;
using FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using FaqAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FaqAssistant.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public async Task<GetUserByIdResponse?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Users.AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => new GetUserByIdResponse(u.Id, u.Username, u.Email)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<GetUserDetailsQueryResponse>> GetUserDetailsAsync(
    int pageNumber,
    int pageSize,
    string? searchValue,
    CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        IQueryable<User> query = _appDbContext.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted);

        // 🔍 Apply search ONLY if value exists
        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            searchValue = searchValue.Trim();

            query = query.Where(u =>
                u.Username.Contains(searchValue) ||
                u.Email.Contains(searchValue));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new GetUserDetailsQueryResponse(
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt))
            .ToListAsync(cancellationToken);

        return PagedResult<GetUserDetailsQueryResponse>.Create(
            users,
            pageNumber,
            pageSize,
            totalCount);
    }



}
