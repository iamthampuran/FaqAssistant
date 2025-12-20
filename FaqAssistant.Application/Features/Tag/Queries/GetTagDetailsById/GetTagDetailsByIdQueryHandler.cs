using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetTagDetailsById;

public class GetTagDetailsByIdQueryHandler : IRequestHandler<GetTagDetailsByIdQuery, Result<GetTagDetailsByIdResponse>>
{
    private readonly ITagRepository _tagRepository;
    public GetTagDetailsByIdQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    }

    public async Task<Result<GetTagDetailsByIdResponse>> Handle(GetTagDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var tagDetails = await _tagRepository.GetTagDetailsById(request.Id, cancellationToken);
        if (tagDetails == null)
        {
            return new Result<GetTagDetailsByIdResponse>(false, "Tag not found.");
        }
        return new Result<GetTagDetailsByIdResponse>(true, tagDetails);
    }
}
