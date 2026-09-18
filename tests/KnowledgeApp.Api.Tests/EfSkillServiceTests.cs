using KnowledgeApp.Api.Data;
using KnowledgeApp.Api.Models;
using KnowledgeApp.Api.Services;
using KnowledgeApp.Api.Skills;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApp.Api.Tests;

public class EfSkillServiceTests
{
    private static EfSkillService CreateService(out SkillAcademyDbContext db, IClaudeSkillFileStore? fileStore = null)
    {
        var options = new DbContextOptionsBuilder<SkillAcademyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        db = new SkillAcademyDbContext(options);
        return new EfSkillService(db, fileStore ?? new NullClaudeSkillFileStore());
    }

    private class FakeClaudeSkillFileStore(IReadOnlyList<DiskSkill> diskSkills) : IClaudeSkillFileStore
    {
        public IReadOnlyList<DiskSkill> ReadAll() => diskSkills;
        public void Write(Skill skill)
        {
        }
        public void Delete(string slug)
        {
        }
        public bool Exists(string slug) => true;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoSkillsExist()
    {
        var service = CreateService(out _);

        var skills = await service.GetAllAsync();

        Assert.Empty(skills);
    }

    [Fact]
    public async Task CreateAsync_AddsSkill_AndAssignsId()
    {
        var service = CreateService(out _);

        var created = await service.CreateAsync(new CreateSkillRequest("Docker", "DevOps", "Package apps consistently.", "Multi-stage Dockerfile.", ["docker"]));

        Assert.True(created.Id > 0);
        Assert.Single(await service.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenSkillDoesNotExist()
    {
        var service = CreateService(out _);

        Assert.Null(await service.GetByIdAsync(999));
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingSkill_AndReturnsTrue()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync(new CreateSkillRequest("Name", "Backend", "Purpose", "HowBuilt", ["tag"]));

        var result = await service.UpdateAsync(created.Id, new UpdateSkillRequest("New Name", "Backend", "New purpose", "New how-built", ["updated"]));

        Assert.True(result);
        var updated = await service.GetByIdAsync(created.Id);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal(["updated"], updated.TechTags);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenSkillDoesNotExist()
    {
        var service = CreateService(out _);

        var result = await service.UpdateAsync(999, new UpdateSkillRequest("Name", "Category", "Purpose", "HowBuilt", null));

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesSkill_AndReturnsTrue()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync(new CreateSkillRequest("Name", "Backend", "Purpose", "HowBuilt", null));

        var result = await service.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenSkillDoesNotExist()
    {
        var service = CreateService(out _);

        Assert.False(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task SyncAsync_SkipsImport_WhenDiskSlugDiffersOnlyByCaseFromExistingSlug()
    {
        var fileStore = new FakeClaudeSkillFileStore([
            new DiskSkill("fix-PR-comments", "Fix PR Comments", "Resolves PR comments.", "body")
        ]);
        var service = CreateService(out var db, fileStore);
        db.Skills.Add(new Skill
        {
            Name = "Fix Pr Comments",
            Category = "Claude Skill",
            Purpose = "Resolves PR comments.",
            HowBuilt = "body",
            TechTags = ["claude-skill"],
            Slug = "fix-pr-comments"
        });
        await db.SaveChangesAsync();

        var result = await service.SyncAsync();

        Assert.Equal(0, result.ImportedFromDisk);
        Assert.Single(await service.GetAllAsync());
    }
}
