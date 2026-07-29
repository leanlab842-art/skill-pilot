using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Abstractions.Persistence;

/// <summary><see cref="UserSkill"/>の永続化を担う。</summary>
public interface IUserSkillRepository
{
    /// <summary>Idと所有者Idで保有スキルを取得する。見つからない、または他人の所有物の場合はnull。</summary>
    Task<UserSkill?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct);

    /// <summary>指定したユーザーの保有スキルを全件取得する。</summary>
    Task<IReadOnlyList<UserSkill>> GetByUserIdAsync(Guid userId, CancellationToken ct);

    /// <summary>指定したユーザーが同名のスキルを既に登録済みかどうかを判定する。</summary>
    Task<bool> ExistsByUserIdAndNameAsync(Guid userId, SkillName skillName, CancellationToken ct);

    /// <summary>新規に保有スキルを追加する。</summary>
    Task AddAsync(UserSkill skill, CancellationToken ct);

    /// <summary>保有スキルを削除する。</summary>
    void Remove(UserSkill skill);
}
