using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;
using FaqAssistant.Application.Features.Tag.Queries.GetTagDetailsById;
using FaqAssistant.Domain.Entities;

namespace FaqAssistant.Application.Interfaces.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<PagedResult<GetTagDetailsResponse>> GetTagDetailsAsync(
       int pageNumber,
       int pageSize,
       string? searchValue,
       CancellationToken cancellationToken);
    Task<GetTagDetailsByIdResponse?> GetTagDetailsById(Guid tagId, CancellationToken cancellationToken);

}
