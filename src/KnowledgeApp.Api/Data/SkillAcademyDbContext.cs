using KnowledgeApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KnowledgeApp.Api.Data;

public class SkillAcademyDbContext(DbContextOptions<SkillAcademyDbContext> options) : DbContext(options)
{
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var techTagsComparer = new ValueComparer<List<string>>(
            (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
            tags => tags.Aggregate(0, (hash, tag) => HashCode.Combine(hash, tag.GetHashCode())),
            tags => tags.ToList());

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Category).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Purpose).IsRequired();
            entity.Property(s => s.HowBuilt).IsRequired();
            entity.Property(s => s.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(s => s.Slug).IsUnique();

            entity
                .Property(s => s.TechTags)
                .HasConversion(
                    tags => string.Join(';', tags),
                    value => value.Length == 0
                        ? new List<string>()
                        : value.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(techTagsComparer);
        });
    }
}
