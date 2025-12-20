using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails;
using FaqAssistant.Application.Features.Faq.Queries.GetFaqDetailsById;
using FaqAssistant.Domain.Entities;

namespace FaqAssistant.Application.Interfaces.Repositories;

public interface IFaqRepository : IGenericRepository<Faq>
{
    // Add any FAQ-specific repository methods here if needed in the future
    Task AddNewTags(List<Guid> tagIds, Guid faqId);

    Task<PagedResult<GetFaqDetailsResponse>> GetFaqDetailsAsync(PageParameters pageParameters, Guid? categoryId, Guid? tagId);

    Task<GetFaqDetailsByIdResponse> GetFaqDetailsByIdAsync(Guid faqId);

}