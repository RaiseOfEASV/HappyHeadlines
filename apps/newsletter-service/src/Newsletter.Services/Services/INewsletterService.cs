using Newsletter.Services.DTOs;

namespace Newsletter.Services.Services;

public interface INewsletterService
{
    Task SendBreakingNewsAsync(Guid articleId, string title, string content, string continent);
    Task SendDailyDigestAsync();
    Task<List<NewsletterDto>> GetHistoryAsync(int limit = 50);
}
