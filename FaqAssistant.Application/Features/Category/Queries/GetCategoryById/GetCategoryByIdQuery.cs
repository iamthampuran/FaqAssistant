using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Queries.GetCategoryById;

public record GetCategoryByIdQuery (Guid Id): IRequest<Result<GetCategoryByIdResponse>>;
