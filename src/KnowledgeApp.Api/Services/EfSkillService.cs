using KnowledgeApp.Api.Data;
using KnowledgeApp.Api.Models;
using KnowledgeApp.Api.Skills;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApp.Api.Services;

public class EfSkillService(SkillAcademyDbContext db, IClaudeSkillFileStore skillFiles) : ISkillService
{
    public async Task<IReadOnlyList<Skill>> GetAllAsync() =>
        await db.Skills.AsNoTracking().OrderBy(s => s.Id).ToListAsync();

    public async Task<Skill?> GetByIdAsync(int id) =>
        await db.Skills.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Skill> CreateAsync(CreateSkillRequest request)
    {
        var skill = new Skill
        {
            Name = request.Name,
            Category = request.Category,
            Purpose = request.Purpose,
            HowBuilt = request.HowBuilt,
            TechTags = request.TechTags ?? [],
            Slug = await GenerateUniqueSlugAsync(request.Name)
        };

        db.Skills.Add(skill);
        await db.SaveChangesAsync();

        skillFiles.Write(skill);
        return skill;
    }

    public async Task<bool> UpdateAsync(int id, UpdateSkillRequest request)
    {
        var skill = await db.Skills.FirstOrDefaultAsync(s => s.Id == id);
        if (skill is null)
        {
            return false;
        }

        skill.Name = request.Name;
        skill.Category = request.Category;
        skill.Purpose = request.Purpose;
        skill.HowBuilt = request.HowBuilt;
        skill.TechTags = request.TechTags ?? [];

        await db.SaveChangesAsync();

        skillFiles.Write(skill);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var skill = await db.Skills.FirstOrDefaultAsync(s => s.Id == id);
        if (skill is null)
        {
            return false;
        }

        db.Skills.Remove(skill);
        await db.SaveChangesAsync();

        skillFiles.Delete(skill.Slug);
        return true;
    }

    public async Task<SyncResult> SyncAsync()
    {
        var existingSlugs = new HashSet<string>(
            await db.Skills.Select(s => s.Slug).ToListAsync(),
            StringComparer.OrdinalIgnoreCase);
        var diskSkills = skillFiles.ReadAll();

        var imported = 0;
        foreach (var disk in diskSkills)
        {
            if (existingSlugs.Contains(disk.Slug))
            {
                continue;
            }

            db.Skills.Add(new Skill
            {
                Name = Titleize(disk.Name ?? disk.Slug),
                Category = "Claude Skill",
                Purpose = disk.Description ?? "",
                HowBuilt = disk.Body.Trim(),
                TechTags = ["claude-skill"],
                Slug = disk.Slug
            });
            imported++;
        }

        if (imported > 0)
        {
            await db.SaveChangesAsync();
        }

        var exported = 0;
        var allSkills = await db.Skills.ToListAsync();
        foreach (var skill in allSkills)
        {
            if (skillFiles.Exists(skill.Slug))
            {
                continue;
            }

            skillFiles.Write(skill);
            exported++;
        }

        return new SyncResult(imported, exported);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name)
    {
        var baseSlug = SlugGenerator.Generate(name);
        var slug = baseSlug;
        var suffix = 2;

        while (await db.Skills.AnyAsync(s => s.Slug == slug))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private static string Titleize(string slug) =>
        string.Join(' ', slug.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
}
