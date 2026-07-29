using SkillPilot.Domain.Entities;
using SkillPilot.Domain.Enums;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Domain.Services;

/// <summary>
/// AIが抽出した必要スキルと、ユーザーの保有スキルを比較し、不足スキル・マッチ率を算出する。
/// </summary>
/// <remarks>
/// <see cref="JobAnalysis"/>(必要スキル側)と<see cref="User"/>(保有スキル側)という
/// 2つの集約をまたぐ横断的なロジックのため、どちらのエンティティにも属させず
/// Domain Serviceとして切り出している。インスタンスの状態を一切持たない純粋な計算のため
/// static メソッドとして提供する(DIコンテナへの登録は不要で、呼び出し側は
/// <c>SkillGapCalculator.Calculate(...)</c>と直接呼び出す)。
/// </remarks>
public static class SkillGapCalculator
{
    /// <summary>
    /// 必要スキル一覧と保有スキル一覧から、不足判定済みの<see cref="SkillResult"/>と
    /// <see cref="MatchRate"/>を算出する。
    /// </summary>
    /// <param name="requiredSkills">AIが抽出した必要スキル一覧。</param>
    /// <param name="userSkills">ユーザーの保有スキル一覧。</param>
    /// <returns>不足判定済みのSkillResult一覧と、必須スキルの充足率。</returns>
    public static (IReadOnlyList<SkillResult> SkillResults, MatchRate MatchRate) Calculate(
        IReadOnlyList<RequiredSkillInput> requiredSkills,
        IReadOnlyList<UserSkill> userSkills)
    {
        var ownedNames = userSkills
            .Select(s => s.SkillName.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var results = requiredSkills
            .Select(s => new SkillResult(s.Name, s.Level, s.Category, isMissing: !ownedNames.Contains(s.Name.Value)))
            .ToList();

        // マッチ率 = 必須(Required)スキルのうち保有しているものの割合
        var requiredCount = results.Count(r => r.Category == SkillCategory.Required);
        var matchedCount = results.Count(r => r.Category == SkillCategory.Required && !r.IsMissing);
        var rate = requiredCount == 0 ? 100 : (int)Math.Round(matchedCount * 100.0 / requiredCount);

        return (results, MatchRate.Create(rate));
    }
}
