using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetAllTags;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, Result<IReadOnlyList<GetAllTagsResponse>>>
{
    private readonly ITagRepository _tagRepository;
    public GetAllTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    }

    public async Task<Result<IReadOnlyList<GetAllTagsResponse>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAsync(t => !t.IsDeleted);
        var response = tags
            .Select(t => new GetAllTagsResponse(t.Id, t.Name, t.CreatedAt))
            .ToList()
            .AsReadOnly();
        var result = new Result<IReadOnlyList<GetAllTagsResponse>>
        {
            Data = response,
            Success = true
        };
        return result;
    }
}
