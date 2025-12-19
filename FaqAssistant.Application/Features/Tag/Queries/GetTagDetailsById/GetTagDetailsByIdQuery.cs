using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetTagDetailsById;

public record GetTagDetailsByIdQuery(Guid Id) : IRequest<Result<GetTagDetailsByIdResponse>>;
