using Microsoft.EntityFrameworkCore;
using Newsletter.Data.entities;

namespace Newsletter.Data.configuration;

public class NewsletterDbContext : DbContext
{
    public NewsletterDbContext(DbContextOptions<NewsletterDbContext> options) : base(options) { }

    public DbSet<SubscriberEntity> Subscribers { get; set; }
    public DbSet<NewsletterHistoryEntity> NewsletterHistory { get; set; }
    public DbSet<NewsletterDeliveryEntity> NewsletterDelivery { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SubscriberEntity configuration
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

        // NewsletterHistoryEntity configuration
        modelBuilder.Entity<NewsletterHistoryEntity>(entity =>
        {
            entity.ToTable("newsletter_history");

            entity.HasKey(e => e.NewsletterId);
            entity.Property(e => e.NewsletterId).HasColumnName("newsletter_id");
            entity.Property(e => e.NewsletterType).HasColumnName("newsletter_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Subject).HasColumnName("subject").IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentHtml).HasColumnName("content_html").IsRequired();
            entity.Property(e => e.ArticleIds).HasColumnName("article_ids").IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.SentAt).HasColumnName("sent_at").IsRequired();
            entity.Property(e => e.RecipientCount).HasColumnName("recipient_count").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.NewsletterType).HasDatabaseName("idx_newsletter_history_type");
            entity.HasIndex(e => e.SentAt).HasDatabaseName("idx_newsletter_history_sent_at");
        });

        // NewsletterDeliveryEntity configuration
        modelBuilder.Entity<NewsletterDeliveryEntity>(entity =>
        {
            entity.ToTable("newsletter_delivery");

            entity.HasKey(e => e.DeliveryId);
            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
            entity.Property(e => e.NewsletterId).HasColumnName("newsletter_id").IsRequired();
            entity.Property(e => e.SubscriberId).HasColumnName("subscriber_id").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(50);
            entity.Property(e => e.SentAt).HasColumnName("sent_at");
            entity.Property(e => e.OpenedAt).HasColumnName("opened_at");
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.NewsletterId).HasDatabaseName("idx_newsletter_delivery_newsletter_id");
            entity.HasIndex(e => e.SubscriberId).HasDatabaseName("idx_newsletter_delivery_subscriber_id");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_newsletter_delivery_status");
        });
    }
}
