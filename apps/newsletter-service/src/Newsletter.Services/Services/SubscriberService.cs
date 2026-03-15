using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsletter.Data.configuration;
using Newsletter.Data.entities;
using Newsletter.Services.DTOs;

namespace Newsletter.Services.Services;

public class SubscriberService : ISubscriberService
{
    private readonly NewsletterDbContext _context;
    private readonly ILogger<SubscriberService> _logger;

    public SubscriberService(NewsletterDbContext context, ILogger<SubscriberService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SubscriberDto> SubscribeAsync(CreateSubscriberDto dto)
    {
        // Check if already exists
        var existing = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == dto.Email);
        if (existing != null)
        {
            if (!existing.IsActive)
            {
                // Reactivate
                existing.IsActive = true;
                existing.SubscribedAt = DateTime.UtcNow;
                existing.UnsubscribedAt = null;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Reactivated subscriber {Email}", dto.Email);
            }
            return MapToDto(existing);
        }

        var subscriber = new SubscriberEntity
        {
            SubscriberId = Guid.NewGuid(),
            Email = dto.Email,
            Name = dto.Name,
            IsActive = true,
            Preferences = JsonSerializer.Serialize(dto.Preferences ?? new Dictionary<string, object>()),
            SubscribedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Subscribers.Add(subscriber);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New subscriber created {Email}", dto.Email);
        return MapToDto(subscriber);
    }

    public async Task<bool> UnsubscribeAsync(string email)
    {
        var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
        if (subscriber == null || !subscriber.IsActive)
        {
            return false;
        }

        subscriber.IsActive = false;
        subscriber.UnsubscribedAt = DateTime.UtcNow;
        subscriber.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Subscriber unsubscribed {Email}", email);
        return true;
    }

    public async Task<SubscriberDto?> UpdatePreferencesAsync(string email, UpdatePreferencesDto dto)
    {
        var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
        if (subscriber == null)
        {
            return null;
        }

        subscriber.Preferences = JsonSerializer.Serialize(dto.Preferences);
        subscriber.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated preferences for {Email}", email);
        return MapToDto(subscriber);
    }

    public async Task<SubscriberDto?> GetByEmailAsync(string email)
    {
        var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
        return subscriber == null ? null : MapToDto(subscriber);
    }

    public async Task<List<SubscriberDto>> GetActiveSubscribersAsync()
    {
        var subscribers = await _context.Subscribers
            .Where(s => s.IsActive)
            .ToListAsync();

        return subscribers.Select(MapToDto).ToList();
    }

    private static SubscriberDto MapToDto(SubscriberEntity entity)
    {
        var preferences = new Dictionary<string, object>();
        try
        {
            preferences = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.Preferences) ?? new();
        }
        catch
        {
            // If deserialization fails, return empty dict
        }

        return new SubscriberDto
        {
            SubscriberId = entity.SubscriberId,
            Email = entity.Email,
            Name = entity.Name,
            IsActive = entity.IsActive,
            Preferences = preferences,
            SubscribedAt = entity.SubscribedAt,
            UnsubscribedAt = entity.UnsubscribedAt
        };
    }
}
