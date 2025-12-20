using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<IReadOnlyList<GetAllCategoriesResponse>>>
{
    private readonly ICategoryRepository _categoryRepository;
    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<Result<IReadOnlyList<GetAllCategoriesResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAsync(c => !c.IsDeleted);
        var response = categories
            .Select(c => new GetAllCategoriesResponse(c.Id, c.Name, c.CreatedAt))
            .ToList()
            .AsReadOnly();
        var result = new Result<IReadOnlyList<GetAllCategoriesResponse>>
        {
            Data = response,
            Success = true
        };
        return result;
    }
}
