using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Category.Queries.GetCategoryDetails;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using FaqAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FaqAssistant.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _appDbContext;
    public CategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
        _appDbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<GetCategoryDetailsResponse>> GetCategoryDetailsAsync(
    int pageNumber,
    int pageSize,
    string? searchValue,
    CancellationToken cancellationToken)
    {
        var query = _appDbContext.Categories.AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            query = query.Where(t => t.Name.Contains(searchValue.Trim()));
        }
        var totalCount = await query.CountAsync(cancellationToken);

        var categories = await query
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new GetCategoryDetailsResponse(t.Id, t.Name, t.CreatedAt))
            .ToListAsync(cancellationToken);

        return PagedResult<GetCategoryDetailsResponse>.Create(
            categories,
            pageNumber,
            pageSize,
            totalCount);
    }
}