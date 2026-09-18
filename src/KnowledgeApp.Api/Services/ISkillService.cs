using KnowledgeApp.Api.Models;

namespace KnowledgeApp.Api.Services;

public record SyncResult(int ImportedFromDisk, int ExportedToDisk);

public interface ISkillService
{
    Task<IReadOnlyList<Skill>> GetAllAsync();
    Task<Skill?> GetByIdAsync(int id);
    Task<Skill> CreateAsync(CreateSkillRequest request);
    Task<bool> UpdateAsync(int id, UpdateSkillRequest request);
    Task<bool> DeleteAsync(int id);
    Task<SyncResult> SyncAsync();
}
