using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetFirstAsync(c => c.Id == request.Id || c.Name == request.Name, cancellationToken);
        if (category == null || category.IsDeleted)
        {
            return new Result<Guid>(false, "Category not found.");
        }
        else if (category.Id != request.Id)
        {
            return new Result<Guid>(false, "Category with the same name already exists.");
        }

        category.Name = request.Name;
        category.LastUpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Result<Guid>(true, category.Id);
    }
}