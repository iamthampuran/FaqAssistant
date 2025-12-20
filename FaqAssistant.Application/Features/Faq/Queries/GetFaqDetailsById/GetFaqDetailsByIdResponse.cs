namespace FaqAssistant.Application.Features.Faq.Queries.GetFaqDetailsById;

public record GetFaqDetailsByIdResponse(Guid Id, string Question, string Answer, FaqCategoryDto Category, List<FaqTagsDto> Tags, DateTime CreatedAt, int Rating, FaqUserDto UserDetails);

public record FaqTagsDto(Guid Id, string Name);
public record FaqCategoryDto(Guid Id, string Name);
public record FaqUserDto(Guid Id, string Username);

