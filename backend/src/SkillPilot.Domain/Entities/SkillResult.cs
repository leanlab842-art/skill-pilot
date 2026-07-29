using SkillPilot.Domain.Common;
using SkillPilot.Domain.Enums;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Domain.Entities;

/// <summary>
/// 求人が求めるスキルと、ユーザーの保有スキルとの充足状況を保持する。
/// 「必要スキル」と「不足スキル」を1つの型で表現し、<see cref="IsMissing"/>で区別する。
/// </summary>
public sealed class SkillResult : BaseEntity
{
    /// <summary>紐づく求人分析のId。</summary>
    public Guid AnalysisId { get; private set; }

    /// <summary>紐づく求人分析への参照ナビゲーション。</summary>
    public JobAnalysis Analysis { get; private set; } = null!;

    /// <summary>スキル名。</summary>
    public SkillName SkillName { get; private set; } = null!;

    /// <summary>求人が求める習熟レベル。</summary>
    public SkillLevel Level { get; private set; }

    /// <summary>必須スキルか歓迎スキルかの区分。</summary>
    public SkillCategory Category { get; private set; }

    /// <summary>ユーザーの保有スキルに存在しない場合はtrue。<see cref="Services.SkillGapCalculator"/>が算出する。</summary>
    public bool IsMissing { get; private set; }

    private SkillResult()
    {
    }

    // SkillGapCalculator(Domain Service)からのみ生成させるため internal とする。
    // Application層はRequiredSkillInputを渡すだけで、SkillResultを直接構築することはない。
    internal SkillResult(SkillName skillName, SkillLevel level, SkillCategory category, bool isMissing)
    {
        SkillName = skillName;
        Level = level;
        Category = category;
        IsMissing = isMissing;
    }
}
