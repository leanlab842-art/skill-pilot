using SkillPilot.Domain.Enums;

namespace SkillPilot.Application.Abstractions.Ai;

/// <summary>AIによる求人分析の結果(生データ)。</summary>
/// <param name="Skills">抽出された必要スキルの一覧。</param>
/// <param name="Roadmap">提案された学習プランの一覧。</param>
public sealed record JobSkillAnalysisResult(
    IReadOnlyList<ExtractedSkill> Skills,
    IReadOnlyList<SuggestedRoadmapItem> Roadmap);

/// <summary>AIが抽出した必要スキル1件分(未検証の生データ)。</summary>
/// <param name="Name">スキル名(自由記述のためUseCase側で<c>SkillName.Create</c>による検証が必要)。</param>
/// <param name="Level">求人が求める習熟レベル。</param>
/// <param name="Category">必須/歓迎の区分。</param>
public sealed record ExtractedSkill(string Name, SkillLevel Level, SkillCategory Category);

/// <summary>AIが提案した学習プラン項目1件分(未検証の生データ)。</summary>
/// <param name="Title">学習項目のタイトル。</param>
/// <param name="Description">補足説明。</param>
/// <param name="Week">何週目の学習項目か。</param>
/// <param name="RelatedSkillName">
/// 対応する不足スキルのスキル名(あれば)。UseCase側で<see cref="ExtractedSkill"/>と突き合わせ、
/// 対応する<c>SkillResult</c>のIdに変換する。
/// </param>
public sealed record SuggestedRoadmapItem(string Title, string? Description, int Week, string? RelatedSkillName);
