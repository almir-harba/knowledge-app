using System.Text;

namespace KnowledgeApp.Api.Skills;

public record ParsedSkillMarkdown(string? Name, string? Description, string Body);

public static class SkillMarkdown
{
    public static ParsedSkillMarkdown Parse(string content)
    {
        if (!content.StartsWith("---", StringComparison.Ordinal))
        {
            return new ParsedSkillMarkdown(null, null, content);
        }

        var closingIndex = content.IndexOf("\n---", 3, StringComparison.Ordinal);
        if (closingIndex < 0)
        {
            return new ParsedSkillMarkdown(null, null, content);
        }

        var frontmatter = content[3..closingIndex];
        var body = content[(closingIndex + 4)..].TrimStart('\r', '\n');

        string? name = null;
        string? description = null;

        foreach (var line in frontmatter.Split('\n'))
        {
            var separatorIndex = line.IndexOf(':');
            if (separatorIndex < 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');

            if (key == "name")
            {
                name = value;
            }
            else if (key == "description")
            {
                description = value;
            }
        }

        return new ParsedSkillMarkdown(name, description, body);
    }

    public static string Build(string slug, string title, string description, string body, IReadOnlyList<string> tags)
    {
        var sb = new StringBuilder();
        sb.Append("---\n");
        sb.Append("name: ").Append(slug).Append('\n');
        sb.Append("description: ").Append(EscapeYamlScalar(description)).Append('\n');
        sb.Append("---\n\n");
        sb.Append("# ").Append(title).Append("\n\n");
        sb.Append(body.Trim()).Append('\n');

        if (tags.Count > 0)
        {
            sb.Append("\n**Tech:** ").Append(string.Join(", ", tags)).Append('\n');
        }

        return sb.ToString();
    }

    private static string EscapeYamlScalar(string value) =>
        value.Contains(':') || value.Contains('"') || value.Contains('#')
            ? $"\"{value.Replace("\"", "\\\"")}\""
            : value;
}
