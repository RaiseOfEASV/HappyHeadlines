using Microsoft.EntityFrameworkCore;
using Publisher.Data.entities;

namespace Publisher.Data.configuration;

public class PublisherDbContext : DbContext
{
    public PublisherDbContext(DbContextOptions<PublisherDbContext> options) : base(options) { }

    public DbSet<PublicationEntity> Publications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PublicationEntity>(entity =>
        {
            entity.ToTable("publications");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DraftId).HasColumnName("draft_id").IsRequired();
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.PublisherId).HasColumnName("publisher_id").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.Continent).HasColumnName("continent").HasMaxLength(50);
            entity.Property(e => e.PublishInitiatedAt).HasColumnName("publish_initiated_at").IsRequired();
            entity.Property(e => e.PublishCompletedAt).HasColumnName("publish_completed_at");
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(e => e.DraftId).HasDatabaseName("idx_publications_draft_id");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_publications_status");
            entity.HasIndex(e => e.PublisherId).HasDatabaseName("idx_publications_publisher_id");
        });
    }
}
