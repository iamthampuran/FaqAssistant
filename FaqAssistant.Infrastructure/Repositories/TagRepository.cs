using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using FaqAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FaqAssistant.Infrastructure.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    private readonly AppDbContext _appDbContext;
    public TagRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public async Task<PagedResult<GetTagDetailsQueryResponse>> GetTagDetailsAsync(
        int pageNumber,
        int pageSize,
        string? searchValue,
        CancellationToken cancellationToken)
    {
        var query = _appDbContext.Tags.AsNoTracking()
            .Where(t => !t.IsDeleted);

        if(!string.IsNullOrWhiteSpace(searchValue))
        {
            query = query.Where(t => t.Name.Contains(searchValue.Trim()));
        }
        var totalCount = await query.CountAsync(cancellationToken);

        var tags = await query
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new GetTagDetailsQueryResponse(t.Id, t.Name, t.CreatedAt))
            .ToListAsync(cancellationToken);

        return PagedResult<GetTagDetailsQueryResponse>.Create(
            tags,
            pageNumber,
            pageSize,
            totalCount);
    }
}
