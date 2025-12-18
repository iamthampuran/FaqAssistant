using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetAllTags;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, Result<IReadOnlyList<GetAllTagsQueryResponse>>>
{
    private readonly ITagRepository _tagRepository;
    public GetAllTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    }

    public async Task<Result<IReadOnlyList<GetAllTagsQueryResponse>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAsync(t => !t.IsDeleted);
        var response = tags
            .Select(t => new GetAllTagsQueryResponse(t.Id, t.Name, t.CreatedAt))
            .ToList()
            .AsReadOnly();
        var result = new Result<IReadOnlyList<GetAllTagsQueryResponse>>
        {
            Data = response,
            Success = true,
            Message = "Tags retrieved successfully."
        };
        return result;
    }
}
