using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Queries.GetFaqDetailsById;

public record GetFaqDetailsByIdQuery (Guid FaqId) : IRequest<Result<GetFaqDetailsByIdResponse>>;
