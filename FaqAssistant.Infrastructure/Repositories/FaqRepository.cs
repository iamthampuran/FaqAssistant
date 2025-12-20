using FaqAssistant.Application.Common;
using FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails;
using FaqAssistant.Application.Features.Faq.Queries.GetFaqDetailsById;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using FaqAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FaqCategoryDto = FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails.FaqCategoryDto;
using FaqTagsDto = FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails.FaqTagsDto;

namespace FaqAssistant.Infrastructure.Repositories;

public class FaqRepository : GenericRepository<Faq>, IFaqRepository
{
    public FaqRepository(AppDbContext context) : base(context)
    {
    }
    public async Task AddNewTags(List<Guid> tagIds, Guid faqId)
    {
        var listOfTags = tagIds.Select(tId => new FaqTag()
        {
            FaqId = faqId,
            TagId = tId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        });
        await _dbContext.FaqTags.AddRangeAsync(listOfTags);
    }

    public async Task<PagedResult<GetFaqDetailsResponse>> GetFaqDetailsAsync(PageParameters pageParameters, Guid? categoryId, Guid? tagId)
    {
        var query = _dbContext.Faqs.AsNoTracking()
            .Include(faq => faq.User)
            .Include(faq => faq.Category)
            .Include(faq => faq.Tags)
            .ThenInclude(ft => ft.Tag)
            .Include(faq => faq.Ratings) as IQueryable<Faq>;
        
        if (categoryId.HasValue)
        {
            query = query.Where(faq => faq.CategoryId == categoryId.Value && !faq.Category.IsDeleted);
        }
        if (tagId.HasValue)
        {
            query = query.Where(faq => faq.Tags.Any(ft => ft.TagId == tagId.Value && !ft.IsDeleted && !ft.Tag.IsDeleted));
        }
        if (!string.IsNullOrWhiteSpace(pageParameters.SearchValue))
        {
            query = query.Where(faq => faq.Question.Contains(pageParameters.SearchValue) || faq.Answer.Contains(pageParameters.SearchValue) || 
            faq.Category.Name.Contains(pageParameters.SearchValue) || faq.Tags.Any(t => t.Tag.Name.Contains(pageParameters.SearchValue)));
        }
        query = query.Where(faq => !faq.IsDeleted).Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize);

        var totalCount = await query.CountAsync();
        var result = await query.Select(faq => new GetFaqDetailsResponse(faq.Id, faq.Question, faq.Answer,
            new FaqCategoryDto(faq.Category.Id, faq.Category.Name),
            faq.Tags.Where(t => !t.IsDeleted).Select(t => new FaqTagsDto(t.Tag.Id, t.Tag.Name)).ToList(),
            faq.CreatedAt, faq.Ratings.Where(r => !r.IsDeleted).Sum(r => r.IsUpvote ? 1 : -1),
            new Application.Features.Faq.Queries.GetFaqDetails.FaqUserDto(faq.User.Id, faq.User.Username)))
            .ToListAsync();

        return PagedResult<GetFaqDetailsResponse>.Create(result, pageParameters.PageNumber, pageParameters.PageSize, totalCount);
    }

    public async Task<GetFaqDetailsByIdResponse> GetFaqDetailsByIdAsync(Guid faqId)
    {
        var faq = await _dbContext.Faqs.AsNoTracking()
            .Include(f => f.User)
            .Include(f => f.Category)
            .Include(f => f.Tags)
            .ThenInclude(ft => ft.Tag)
            .Include(f => f.Ratings)
            .FirstOrDefaultAsync(f => f.Id == faqId && !f.IsDeleted);
        
        if (faq == null)
        {
            throw new KeyNotFoundException($"FAQ with ID {faqId} not found.");
        }
        
        return new GetFaqDetailsByIdResponse(
            faq.Id,
            faq.Question,
            faq.Answer,
            new Application.Features.Faq.Queries.GetFaqDetailsById.FaqCategoryDto(faq.Category.Id, faq.Category.Name),
            faq.Tags.Where(t => !t.IsDeleted).Select(t => new Application.Features.Faq.Queries.GetFaqDetailsById.FaqTagsDto(t.Tag.Id, t.Tag.Name)).ToList(),
            faq.CreatedAt,
            faq.Ratings.Where(r => !r.IsDeleted).Sum(r => r.IsUpvote ? 1 : -1),
            new Application.Features.Faq.Queries.GetFaqDetailsById.FaqUserDto(faq.User.Id, faq.User.Username)
        );
    }
}