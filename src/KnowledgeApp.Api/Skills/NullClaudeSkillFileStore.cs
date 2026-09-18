using KnowledgeApp.Api.Models;

namespace KnowledgeApp.Api.Skills;

/// Used in the Testing environment (and in unit tests) so runs never touch the real .claude/skills folder.
public class NullClaudeSkillFileStore : IClaudeSkillFileStore
{
    public IReadOnlyList<DiskSkill> ReadAll() => [];

    public void Write(Skill skill)
    {
    }

    public void Delete(string slug)
    {
    }

    public bool Exists(string slug) => true;
}
