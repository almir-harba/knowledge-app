namespace KnowledgeApp.Api.Models;

public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required string Purpose { get; set; }
    public required string HowBuilt { get; set; }
    public List<string> TechTags { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// Folder/frontmatter name of the real Claude skill this row is backed by (.claude/skills/{Slug}/SKILL.md).
    public string Slug { get; set; } = "";
}

public record CreateSkillRequest(string Name, string Category, string Purpose, string HowBuilt, List<string>? TechTags);

public record UpdateSkillRequest(string Name, string Category, string Purpose, string HowBuilt, List<string>? TechTags);
