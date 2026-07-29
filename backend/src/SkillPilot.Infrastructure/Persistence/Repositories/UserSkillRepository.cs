using Microsoft.EntityFrameworkCore;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IUserSkillRepository" />
public sealed class UserSkillRepository : IUserSkillRepository
{
    private readonly SkillPilotDbContext _db;

    public UserSkillRepository(SkillPilotDbContext db) => _db = db;

    /// <inheritdoc />
    public Task<UserSkill?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct) =>
        // 所有者チェックをクエリ自体に組み込み、他人のスキルを取得できないようにする
        // (api.mdの「他人のリソースは404」を、データアクセス層でも多層防御する)。
        _db.UserSkills.SingleOrDefaultAsync(s => s.Id == id && s.UserId == userId, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserSkill>> GetByUserIdAsync(Guid userId, CancellationToken ct) =>
        await _db.UserSkills.Where(s => s.UserId == userId).ToListAsync(ct);

    /// <inheritdoc />
    public Task<bool> ExistsByUserIdAndNameAsync(Guid userId, SkillName skillName, CancellationToken ct) =>
        _db.UserSkills.AnyAsync(s => s.UserId == userId && s.SkillName == skillName, ct);

    /// <inheritdoc />
    public Task AddAsync(UserSkill skill, CancellationToken ct)
    {
        _db.UserSkills.Add(skill);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(UserSkill skill) => _db.UserSkills.Remove(skill);
}
