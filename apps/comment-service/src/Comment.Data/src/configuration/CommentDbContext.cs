using Comment.Data.entities;
using Microsoft.EntityFrameworkCore;

namespace Comment.Data.configuration;

public class CommentDbContext : DbContext
{
    public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options) { }

    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CommentEntity>(entity =>
        {
            entity.ToTable("comments");
            entity.HasKey(e => e.CommentId);
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id").IsRequired();
            entity.Property(e => e.AuthorId).HasColumnName("author_id").IsRequired();
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        });
    }
}