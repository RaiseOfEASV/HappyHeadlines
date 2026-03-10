using Article.Data.entities;
using Microsoft.EntityFrameworkCore;

namespace Article.Data.configuration;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }
    protected ArticleDbContext(DbContextOptions options) : base(options) { }

    public DbSet<ArticleEntity> Articles { get; set; }
    public DbSet<ArticleAuthor> ArticleAuthors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ArticleEntity>(entity =>
        {
            entity.ToTable("Articles");
            entity.HasKey(e => e.ArticleId);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();

            entity.HasMany(e => e.Authors)
                  .WithOne(a => a.Article)
                  .HasForeignKey(a => a.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ArticleAuthor>(entity =>
        {
            entity.ToTable("ArticleAuthors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArticleId).IsRequired();
            entity.Property(e => e.AuthorId).IsRequired();
        });
    }
}
