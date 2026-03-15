using Microsoft.EntityFrameworkCore;
using Subscriber.Data.entities;

namespace Subscriber.Data.configuration;

public class SubscriberDbContext : DbContext
{
    public SubscriberDbContext(DbContextOptions<SubscriberDbContext> options) : base(options) { }

    public DbSet<SubscriberEntity> Subscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SubscriberEntity>(entity =>
        {
            entity.ToTable("subscribers");

            entity.HasKey(e => e.SubscriberId);
            entity.Property(e => e.SubscriberId).HasColumnName("subscriber_id");
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(e => e.Preferences).HasColumnName("preferences").IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.SubscribedAt).HasColumnName("subscribed_at").IsRequired();
            entity.Property(e => e.UnsubscribedAt).HasColumnName("unsubscribed_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("idx_subscribers_email");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("idx_subscribers_is_active");
        });
    }
}
