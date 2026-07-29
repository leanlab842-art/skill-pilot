using SkillPilot.Domain.Entities;
using SkillPilot.Domain.Enums;

namespace SkillPilot.Application.Common.Dtos;

/// <summary>求人が求めるスキルと、不足判定結果。</summary>
/// <param name="Id">SkillResultId。</param>
/// <param name="SkillName">スキル名。</param>
/// <param name="Level">求人が求める習熟レベル。</param>
/// <param name="Category">必須/歓迎の区分。</param>
/// <param name="IsMissing">ユーザーに不足しているスキルかどうか。</param>
public sealed record SkillResultDto(Guid Id, string SkillName, SkillLevel Level, SkillCategory Category, bool IsMissing)
{
    /// <summary>エンティティからDTOへ変換する。</summary>
    public static SkillResultDto FromEntity(SkillResult entity) =>
        new(entity.Id, entity.SkillName.Value, entity.Level, entity.Category, entity.IsMissing);
}

/// <summary>学習ロードマップの1項目。</summary>
/// <param name="Id">LearningRoadmapId。</param>
/// <param name="SkillResultId">対応する不足スキルのId(あれば)。</param>
/// <param name="Title">学習項目のタイトル。</param>
/// <param name="Description">補足説明。</param>
/// <param name="Week">何週目の学習項目か。</param>
/// <param name="Completed">完了済みかどうか。</param>
public sealed record LearningRoadmapDto(Guid Id, Guid? SkillResultId, string Title, string? Description, int Week, bool Completed)
{
    /// <summary>エンティティからDTOへ変換する。</summary>
    public static LearningRoadmapDto FromEntity(LearningRoadmap entity) =>
        new(entity.Id, entity.SkillResultId, entity.Title, entity.Description, entity.Week, entity.Completed);
}

/// <summary>求人分析の詳細。作成・詳細取得・更新の各UseCaseで共有する。</summary>
/// <param name="Id">求人分析Id。</param>
/// <param name="CompanyName">会社名。</param>
/// <param name="JobTitle">求人タイトル。</param>
/// <param name="JobUrl">求人ページの参照用URL(任意)。</param>
/// <param name="Status">分析の進行状態。</param>
/// <param name="MatchRate">必須スキルの充足率(0〜100)。未分析の場合はnull。</param>
/// <param name="SkillResults">必要スキルと不足判定結果の一覧。</param>
/// <param name="Roadmap">学習ロードマップ。</param>
/// <param name="CreatedAt">作成日時。</param>
/// <param name="UpdatedAt">更新日時。</param>
public sealed record JobAnalysisDetailDto(
    Guid Id,
    string CompanyName,
    string JobTitle,
    string? JobUrl,
    AnalysisStatus Status,
    int? MatchRate,
    IReadOnlyList<SkillResultDto> SkillResults,
    IReadOnlyList<LearningRoadmapDto> Roadmap,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>エンティティからDTOへ変換する。</summary>
    public static JobAnalysisDetailDto FromEntity(JobAnalysis entity) => new(
        entity.Id,
        entity.CompanyName,
        entity.JobTitle,
        entity.JobUrl,
        entity.Status,
        entity.MatchRate?.Value,
        entity.SkillResults.Select(SkillResultDto.FromEntity).ToList(),
        entity.Roadmap.Select(LearningRoadmapDto.FromEntity).ToList(),
        entity.CreatedAt,
        entity.UpdatedAt);
}

/// <summary>求人分析の一覧表示用の要約(オーバーフェッチ防止のためSkillResults/Roadmapを含まない)。</summary>
/// <param name="Id">求人分析Id。</param>
/// <param name="CompanyName">会社名。</param>
/// <param name="JobTitle">求人タイトル。</param>
/// <param name="Status">分析の進行状態。</param>
/// <param name="MatchRate">必須スキルの充足率(0〜100)。未分析の場合はnull。</param>
/// <param name="CreatedAt">作成日時。</param>
/// <param name="UpdatedAt">更新日時。</param>
public sealed record JobAnalysisSummaryDto(Guid Id, string CompanyName, string JobTitle, AnalysisStatus Status, int? MatchRate, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    /// <summary>エンティティからDTOへ変換する。</summary>
    public static JobAnalysisSummaryDto FromEntity(JobAnalysis entity) =>
        new(entity.Id, entity.CompanyName, entity.JobTitle, entity.Status, entity.MatchRate?.Value, entity.CreatedAt, entity.UpdatedAt);
}
