using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;

public class GetTagDetailsQueryHandler : IRequestHandler<GetTagDetailsQuery, Result<PagedResult<GetTagDetailsResponse>>>
{
    private readonly ITagRepository _tagRepository;
    public GetTagDetailsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    }

    public async Task<Result<PagedResult<GetTagDetailsResponse>>> Handle(GetTagDetailsQuery request, CancellationToken cancellationToken)
    {
        var pagedTags = await _tagRepository.GetTagDetailsAsync(request.PageCount, request.PageSize, request.SearchValue, cancellationToken);
        return new Result<PagedResult<GetTagDetailsResponse>>(true, pagedTags);
    }
}
