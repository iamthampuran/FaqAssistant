using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Domain.Entities.Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _categoryRepository.AddAsync(category, cancellationToken);

        return new Result<Guid>(true, category.Id, "Category created successfully.");
    }
}