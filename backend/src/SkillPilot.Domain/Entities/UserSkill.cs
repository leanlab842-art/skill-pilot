using SkillPilot.Domain.Common;
using SkillPilot.Domain.Enums;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Domain.Entities;

/// <summary>
/// ユーザーが自己申告した保有スキル。AI分析における「不足スキル」判定の比較対象になる。
/// </summary>
/// <remarks>
/// APIは<c>/users/me/skills/{id}</c>として本エンティティを直接CRUDする設計のため、
/// <see cref="User"/>を経由しないと操作できない厳格な集約にはしていない
/// (<see cref="JobAnalysis"/>集約とは異なる非対称な設計)。
/// </remarks>
public sealed class UserSkill : BaseEntity
{
    /// <summary>所有者のユーザーId。</summary>
    public Guid UserId { get; private set; }

    /// <summary>所有者への参照ナビゲーション。</summary>
    public User User { get; private set; } = null!;

    /// <summary>スキル名。</summary>
    public SkillName SkillName { get; private set; } = null!;

    /// <summary>習熟レベル。</summary>
    public SkillLevel Level { get; private set; }

    private UserSkill()
    {
    }

    /// <summary>保有スキルを新規登録する。</summary>
    /// <param name="userId">所有者のユーザーId。</param>
    /// <param name="skillName">スキル名。</param>
    /// <param name="level">習熟レベル。</param>
    public UserSkill(Guid userId, SkillName skillName, SkillLevel level)
    {
        UserId = userId;
        SkillName = skillName;
        Level = level;
    }

    /// <summary>習熟レベルを更新する。</summary>
    /// <param name="level">新しい習熟レベル。</param>
    public void UpdateLevel(SkillLevel level)
    {
        Level = level;
        Touch();
    }
}
