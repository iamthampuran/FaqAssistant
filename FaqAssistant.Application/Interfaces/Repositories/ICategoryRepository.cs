using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Category.Queries.GetCategoryDetails;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Domain.Entities;

namespace FaqAssistant.Application.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    public Task<PagedResult<GetCategoryDetailsResponse>> GetCategoryDetailsAsync(
    int pageNumber,
    int pageSize,
    string? searchValue,
    CancellationToken cancellationToken);

}