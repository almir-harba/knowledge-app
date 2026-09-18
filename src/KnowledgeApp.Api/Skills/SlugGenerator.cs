using System.Text;

namespace KnowledgeApp.Api.Skills;

public static class SlugGenerator
{
    public static string Generate(string name)
    {
        var sb = new StringBuilder();
        var lastWasDash = true;

        foreach (var ch in name.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
                lastWasDash = false;
            }
            else if (!lastWasDash)
            {
                sb.Append('-');
                lastWasDash = true;
            }
        }

        return sb.ToString().Trim('-') is { Length: > 0 } slug ? slug : "skill";
    }
}
