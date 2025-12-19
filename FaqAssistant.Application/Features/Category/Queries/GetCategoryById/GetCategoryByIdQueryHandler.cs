using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaqAssistant.Application.Features.Category.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return new Result<GetCategoryByIdResponse>(false, "Category not found.");
        }
        var response = new GetCategoryByIdResponse(category.Id, category.Name, category.CreatedAt);
        return new Result<GetCategoryByIdResponse>(true, response);
    }
}
