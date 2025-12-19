using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Queries.GetCategoryDetails;

public class GetCategoryDetailsQueryHandler : IRequestHandler<GetCategoryDetailsQuery, Result<PagedResult<GetCategoryDetailsResponse>>>
{
    private readonly ICategoryRepository _categoryRepostory;
    public GetCategoryDetailsQueryHandler(ICategoryRepository categoryRepostory)
    {
        _categoryRepostory = categoryRepostory ?? throw new ArgumentNullException(nameof(categoryRepostory));
    }

    public async Task<Result<PagedResult<GetCategoryDetailsResponse>>> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepostory.GetCategoryDetailsAsync(request.PageCount, request.PageSize, request.SearchValue,cancellationToken);

        return new Result<PagedResult<GetCategoryDetailsResponse>>(true, categories);
    }
}
