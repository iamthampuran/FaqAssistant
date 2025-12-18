using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Domain.Entities;

namespace FaqAssistant.Application.Interfaces.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<PagedResult<GetTagDetailsQueryResponse>> GetTagDetailsAsync(
       int pageNumber,
       int pageSize,
       string? searchValue,
       CancellationToken cancellationToken);
}
