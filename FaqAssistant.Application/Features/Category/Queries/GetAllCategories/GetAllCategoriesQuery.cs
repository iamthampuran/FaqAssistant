using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Queries.GetAllCategories;

public record GetAllCategoriesQuery() : IRequest<Result<IReadOnlyList<GetAllCategoriesResponse>>>;
