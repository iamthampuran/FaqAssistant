using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace FaqAssistant.Infrastructure.Services;

public class AskAIService : IAskAIService
{
    private readonly IConfiguration _configuration;
    private readonly IFaqRepository _faqRepository;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;

    public AskAIService(IConfiguration configuration, IFaqRepository faqRepository, HttpClient httpClient, IMemoryCache memoryCache)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task<string?> GetAnswerAsync(Guid faqId, CancellationToken cancellationToken)
    {
        if(_memoryCache.TryGetValue(faqId, out string? cachedAnswer) && !string.IsNullOrEmpty(cachedAnswer))
        {
            return cachedAnswer;
        }
        var faq = await _faqRepository.GetByIdAsync(faqId, cancellationToken);
        if (faq == null)
        {
            return null;
        }
        var apiKey = _configuration["AISettings:Key"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("AI key is not configured.");
        }
        var requestBody = new
        {
            model = "llama-3.3-70b-versatile", // use default or config
            messages = new[]
            {
                new {
                    role = "system",
                    content = "You are a helpful FAQ assistant."
                },
                new {
                    role = "user",
                    content = faq.Question  // user’s question
                }
            },
            temperature = 0.7,
            max_tokens = 512,
            top_p = 1.0
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        try
        {
            var response = await _httpClient.PostAsync(_configuration["AISettings:BaseUrl"], content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(responseContent);
            var answer = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (!string.IsNullOrEmpty(answer))
            {
                _memoryCache.Set(faqId, answer, TimeSpan.FromMinutes(30));
            }
            return answer;
        }
        catch (Exception)
        {
            return null;
        }


    }
}
