using KnowledgeApp.Api.Models;
using KnowledgeApp.Api.Skills;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApp.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(SkillAcademyDbContext db, IClaudeSkillFileStore skillFiles)
    {
        if (await db.Skills.AnyAsync())
        {
            return;
        }

        var seedSkills = new[]
        {
            new Skill
            {
                Name = "Skill Academy API",
                Category = "Backend",
                Purpose = "Practice designing and documenting a REST API: modeling a domain, CRUD endpoints, validation, and DI.",
                HowBuilt = ".NET 10 minimal API backed by SQL Server via EF Core, plus xUnit unit and integration tests.",
                TechTags = ["dotnet", "minimal-api", "efcore", "sqlserver"],
                Slug = "skill-academy-api"
            },
            new Skill
            {
                Name = "Skill Academy UI",
                Category = "Frontend",
                Purpose = "Practice building a modern SPA that lists, filters, and manages records fetched from a REST API.",
                HowBuilt = "Angular (standalone components, signals, routing) styled with Tailwind CSS, calling the API via HttpClient.",
                TechTags = ["angular", "tailwindcss", "typescript"],
                Slug = "skill-academy-ui"
            }
        };

        db.Skills.AddRange(seedSkills);
        await db.SaveChangesAsync();

        foreach (var skill in seedSkills)
        {
            skillFiles.Write(skill);
        }
    }
}
