using System;
using System.Collections.Generic;
using System.Text;

namespace FaqAssistant.Application.Features.Category.Queries.GetCategoryById;

public record GetCategoryByIdResponse(Guid Id, string Name, DateTime CreatedAt);
