using SkillPilot.Domain.Enums;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Domain.Services;

/// <summary>
/// AIが抽出した必要スキル1件分の入力データ。<see cref="SkillGapCalculator"/>への入力として使う。
/// </summary>
/// <param name="Name">スキル名。</param>
/// <param name="Level">求人が求める習熟レベル。</param>
/// <param name="Category">必須/歓迎の区分。</param>
public readonly record struct RequiredSkillInput(SkillName Name, SkillLevel Level, SkillCategory Category);
