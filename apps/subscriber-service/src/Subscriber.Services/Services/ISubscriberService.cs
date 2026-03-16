using Subscriber.Services.DTOs;

namespace Subscriber.Services.Services;

public interface ISubscriberService
{
    Task<SubscriberDto> SubscribeAsync(CreateSubscriberDto dto);
    Task<bool> UnsubscribeAsync(string email);
    Task<SubscriberDto?> UpdatePreferencesAsync(string email, UpdatePreferencesDto dto);
    Task<SubscriberDto?> GetByEmailAsync(string email);
    Task<List<SubscriberDto>> GetActiveSubscribersAsync();
}
