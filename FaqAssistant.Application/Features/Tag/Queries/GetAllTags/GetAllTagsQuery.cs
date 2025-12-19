using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetAllTags;

public class GetAllTagsQuery : IRequest<Result<IReadOnlyList<GetAllTagsResponse>>>
{
}
