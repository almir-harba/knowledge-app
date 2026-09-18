using KnowledgeApp.Api.Models;

namespace KnowledgeApp.Api.Skills;

public record DiskSkill(string Slug, string? Name, string? Description, string Body);

public interface IClaudeSkillFileStore
{
    IReadOnlyList<DiskSkill> ReadAll();
    void Write(Skill skill);
    void Delete(string slug);
    bool Exists(string slug);
}

public class ClaudeSkillFileStore : IClaudeSkillFileStore
{
    private readonly string _root;

    public ClaudeSkillFileStore(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configuredPath = configuration["ClaudeSkills:Directory"] ?? "../../.claude/skills";
        _root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredPath));
    }

    public IReadOnlyList<DiskSkill> ReadAll()
    {
        if (!Directory.Exists(_root))
        {
            return [];
        }

        var results = new List<DiskSkill>();

        foreach (var directory in Directory.GetDirectories(_root))
        {
            var skillFile = Path.Combine(directory, "SKILL.md");
            if (!File.Exists(skillFile))
            {
                continue;
            }

            var parsed = SkillMarkdown.Parse(File.ReadAllText(skillFile));
            var slug = Path.GetFileName(directory);
            results.Add(new DiskSkill(slug, parsed.Name, parsed.Description, parsed.Body));
        }

        return results;
    }

    public void Write(Skill skill)
    {
        var directory = Path.Combine(_root, skill.Slug);
        Directory.CreateDirectory(directory);

        var markdown = SkillMarkdown.Build(skill.Slug, skill.Name, skill.Purpose, skill.HowBuilt, skill.TechTags);
        File.WriteAllText(Path.Combine(directory, "SKILL.md"), markdown);
    }

    public void Delete(string slug)
    {
        var directory = Path.Combine(_root, slug);
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    public bool Exists(string slug) =>
        File.Exists(Path.Combine(_root, slug, "SKILL.md"));
}
