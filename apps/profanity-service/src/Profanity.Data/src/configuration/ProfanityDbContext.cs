using Microsoft.EntityFrameworkCore;
using Profanity.Data.entities;

namespace Profanity.Data.configuration;

public class ProfanityDbContext : DbContext
{
    public ProfanityDbContext(DbContextOptions<ProfanityDbContext> options) : base(options) { }

    public DbSet<ProfanityWordEntity> ProfanityWords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProfanityWordEntity>(entity =>
        {
            entity.ToTable("profanity_words");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Word).HasColumnName("word").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.HasIndex(e => e.Word).IsUnique();
        });
    }
}
